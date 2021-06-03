using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Catalyst.WebJobs.ProductUpload.Models;
using Catalyst.WebJobs.ProductUpload.Mapping;
using OrderCloud.SDK;
using Catalyst.WebJobs.ProductUpload.Extensions;
using Microsoft.Extensions.Logging;
using System.IO;
using Catalyst.Common;
using System.Reflection;
using Newtonsoft.Json;

namespace Catalyst.WebJobs.ProductUpload.Commands
{
    public interface IProductCommand 
    {
        Task ProcessSampleProducts(string file, string token, ILogger logger);
    }

    public class ProductCommand : IProductCommand
    {
        private readonly IOrderCloudClient _oc;

        public ProductCommand(IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task<PriceSchedule> BuildPriceScheduleOC(PriceSchedule ps, string token, ILogger logger)
        {
            try
            {
                Console.WriteLine("PS: " + ps.ID);
                var p = await _oc.PriceSchedules.SaveAsync<PriceSchedule>(ps.ID, ps, token);
                return p;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Price Schedule failed {ex.Message}: {ps.ID}");
                return null;
            }
        }

        public async Task<Product> BuildProductOC(Product product, string token, ILogger logger)
        {
            try
            {
                Console.WriteLine("Product: " + product.ID);
                var p = await _oc.Products.SaveAsync<Product>(product.ID, product, token);
                return p;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Product failed {ex.Message}: {product.ID}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<Spec> BuildSpecOC(Spec spec, string token, ILogger logger)
        {
            try
            {
                Console.WriteLine("Spec: " + spec.ID);
                var s = await _oc.Specs.SaveAsync<Spec>(spec.ID, spec, token);
                return s;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Spec failed {ex.Message}: {spec.ID}");
                return null;
            }
        }

        public async Task<SpecOption> BuildSpecOptionOC(SpecOption option, string token, ILogger logger)
        {
            try
            {
                Console.WriteLine("Option: " + option.ID);
                var o = await _oc.Specs.SaveOptionAsync<SpecOption>(option.xp.SpecID, option.ID, option,
                    token);
                return o;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build spec option failed {ex.Message}: SpecID: {option.xp.SpecID}, OptionID: {option.ID}");
                return null;
            }

        }

        public async Task<SpecProductAssignment> BuildSpecProductAssignmentOC(SpecProductAssignment assn, string token, ILogger logger)
        {
            try
            {
                Console.WriteLine("Spec Product Assignment: " + assn.ProductID);
                await _oc.Specs.SaveProductAssignmentAsync(assn, token);
                return assn;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Spec Product Assignment failed {ex.Message}: Product: {assn.ProductID}, Spec: {assn.SpecID}");
                return null;
            }
        }
        public async Task GenerateOCVariants(string productID, string token, ILogger logger)
        {
            try
            {
                Console.WriteLine("Generate Variants: " + productID);
                await _oc.Products.GenerateVariantsAsync(productID, overwriteExisting: true, accessToken: token);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Generate OC Variants failed {ex.Message}: Product: {productID}");
            }
        }
        public async Task UpdateProductVariantsOC(VariantPlaceholder variant, string token, ILogger logger)
        {
            try
            {
                // Get the variants previously created
                var genertatedVariants = await _oc.Products.ListVariantsAsync(variant.ProductID);

                // Modify the ID for the newly genrated variants.
                foreach (Variant v in genertatedVariants.Items)
                {
                    Console.WriteLine("Update Variant: " + v.ID);
                    Variant ocVariant = await _oc.Products.GetVariantAsync(variant.ProductID, v.ID, token);
                    var ocSpec = await _oc.Specs.GetAsync(variant.SpecID, token);
                    if(ocVariant != null && ocSpec != null)
                    {
                        foreach (SpecOption specOption in ocSpec.Options)
                        {
                            foreach (VariantSpec spec in ocVariant.Specs)
                            {
                                if (spec.OptionID == variant.SpecOptionID)
                                {
                                    ocVariant.ID = variant.VariantSKU;
                                }
                            }
                        }
                        await _oc.Products.SaveVariantAsync(variant.ProductID, v.ID, ocVariant, token);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Product Variants failed {ex.Message}: Product: {variant.ProductID}");
            }
        }

        public async Task ProcessSampleProducts(string filename, string token, ILogger logger)
        {
            Console.WriteLine("Processing Sample Products");
            List<PriceSchedule> prices = new List<PriceSchedule>();
            List<Product> products = new List<Product>();
            List<Spec> specs = new List<Spec>();
            List<SpecOption> specOptions = new List<SpecOption>();
            List<SpecProductAssignment> specProductAssignments = new List<SpecProductAssignment>();
            List<VariantPlaceholder> variantPlaceholders = new List<VariantPlaceholder>();


            // Map Customer Provided Product Data to OrderCloud Headstart Classes
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));
            SampleProductList productsFromFile = JsonConvert.DeserializeObject<SampleProductList>(File.ReadAllText(rootDirectory + filename));
            
            foreach(SampleProduct product in productsFromFile.products)
            {
                // Build list of Price Schedules
                var priceSchedule = ProductMapping.MapOCPriceSchedule(product);
                prices.Add(priceSchedule);

                // Build list of Products
                var tempProduct = ProductMapping.MapOCProduct(product);
                products.Add(tempProduct);

                // Build Specs if more than 1 product in group
                if(product.options.Count > 0)
                {
                    foreach (SampleOption option in product.options)
                    {
                        var tempSpec = ProductMapping.MapOCProductSpec(option);
                        specs.Add(tempSpec);

                        foreach(string val in option.values)
                        {
                            var tempOption = ProductMapping.MapOCProductSpecOption(product.id, option.name, val);
                            specOptions.Add(tempOption);

                            // The sample data provided will generate a simple list of variants based solely on color options.  
                            // Real world products will require more robust data mapping for variants if the need to modify them exists.
                            var tempVariant = ProductMapping.MapOCProductVariant(product, option, val);
                            variantPlaceholders.Add(tempVariant);
                        }

                        var tempSPA = ProductMapping.MapOCProductSpecAssignment(product.id, option.name);
                        specProductAssignments.Add(tempSPA);
                    }
                }

            }

            // Build PriceShcedule
            await Throttler.RunAsync(prices, 100, 20, price => BuildPriceScheduleOC(price, token, logger));

            // Build Product
            await Throttler.RunAsync(products, 100, 20, product => BuildProductOC(product, token, logger));

            if(specs.Count > 0)
            {
                // Build Specs
                await Throttler.RunAsync(specs, 100, 20, spec => BuildSpecOC(spec, token, logger));

                // Build Spec Options
                await Throttler.RunAsync(specOptions, 100, 20, specoption => BuildSpecOptionOC(specoption, token, logger));

                // Assign Specs to Product
                await Throttler.RunAsync(specProductAssignments, 100, 20, specprodassignment => BuildSpecProductAssignmentOC(specprodassignment, token, logger));

                // Generate Variants
                var variantGroups = variantPlaceholders.GroupBy(v => v.ProductID);
                foreach (IList<VariantPlaceholder> variantGroup in variantGroups)
                {
                    // Allow the system to generate variants based on selection specs
                    await GenerateOCVariants(variantGroup[0].ProductID, token, logger);

                    //Modify the generated specs to use custom variant id's
                    await Throttler.RunAsync(variantGroup, 500, 1, variant => UpdateProductVariantsOC(variant, token, logger));
                }
            }
        }
    }
}
