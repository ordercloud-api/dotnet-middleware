using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OrderCloud.SDK;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Catalyst.Common.ProductUpload.Models;
using OrderCloud.Catalyst;
using Catalyst.Common.ProductUpload.Mapping;

namespace Catalyst.Common.ProductUpload.Commands
{
    public interface IProductCommand 
    {
        Task ProcessSampleProducts(string file, ILogger logger);
    }

    public class ProductCommand : IProductCommand
    {
        private readonly IOrderCloudClient _oc;

        public ProductCommand(IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task<PriceSchedule> BuildPriceScheduleOC(PriceSchedule ps, ILogger logger)
        {
            try
            {
                logger.LogInformation("PS: " + ps.ID);
                var p = await _oc.PriceSchedules.SaveAsync<PriceSchedule>(ps.ID, ps);
                return p;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Price Schedule failed {ex.Message}: {ps.ID}");
                return null;
            }
        }

        public async Task<Product> BuildProductOC(Product product, ILogger logger)
        {
            try
            {
                logger.LogInformation("Product: " + product.ID);
                var p = await _oc.Products.SaveAsync<Product>(product.ID, product);
                return p;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Product failed {ex.Message}: {product.ID}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<Spec> BuildSpecOC(Spec spec, ILogger logger)
        {
            try
            {
                logger.LogInformation("Spec: " + spec.ID);
                var s = await _oc.Specs.SaveAsync<Spec>(spec.ID, spec);
                return s;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Spec failed {ex.Message}: {spec.ID}");
                return null;
            }
        }

        public async Task<SpecOption> BuildSpecOptionOC(SpecOption option, ILogger logger)
        {
            try
            {
                logger.LogInformation("Option: " + option.ID);
                var o = await _oc.Specs.SaveOptionAsync<SpecOption>(option.xp.SpecID, option.ID, option);
                return o;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build spec option failed {ex.Message}: SpecID: {option.xp.SpecID}, OptionID: {option.ID}");
                return null;
            }

        }

        public async Task<SpecProductAssignment> BuildSpecProductAssignmentOC(SpecProductAssignment assn, ILogger logger)
        {
            try
            {
                logger.LogInformation("Spec Product Assignment: " + assn.ProductID);
                await _oc.Specs.SaveProductAssignmentAsync(assn);
                return assn;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Spec Product Assignment failed {ex.Message}: Product: {assn.ProductID}, Spec: {assn.SpecID}");
                return null;
            }
        }
        public async Task GenerateOCVariants(string productID, ILogger logger)
        {
            try
            {
                logger.LogInformation("Generate Variants: " + productID);
                await _oc.Products.GenerateVariantsAsync(productID, overwriteExisting: true);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Generate OC Variants failed {ex.Message}: Product: {productID}");
            }
        }
        public async Task UpdateProductVariantsOC(VariantPlaceholder variant, ILogger logger)
        {
            try
            {
                // Get the variants previously created
                var genertatedVariants = await _oc.Products.ListVariantsAsync(variant.ProductID);

                // Modify the ID for the newly genrated variants.
                foreach (Variant v in genertatedVariants.Items)
                {
                    logger.LogInformation("Update Variant: " + v.ID);
                    Variant ocVariant = await _oc.Products.GetVariantAsync(variant.ProductID, v.ID);
                    var ocSpec = await _oc.Specs.GetAsync(variant.SpecID);
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
                        await _oc.Products.SaveVariantAsync(variant.ProductID, v.ID, ocVariant);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Build Product Variants failed {ex.Message}: Product: {variant.ProductID}");
            }
        }

        public async Task ProcessSampleProducts(string filename, ILogger logger)
        {
            logger.LogInformation("Processing Sample Products");
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
            await Throttler.RunAsync(prices, 100, 20, price => BuildPriceScheduleOC(price, logger));

            // Build Product
            await Throttler.RunAsync(products, 100, 20, product => BuildProductOC(product, logger));

            if(specs.Count > 0)
            {
                // Build Specs
                await Throttler.RunAsync(specs, 100, 20, spec => BuildSpecOC(spec, logger));

                // Build Spec Options
                await Throttler.RunAsync(specOptions, 100, 20, specoption => BuildSpecOptionOC(specoption, logger));

                // Assign Specs to Product
                await Throttler.RunAsync(specProductAssignments, 100, 20, specprodassignment => BuildSpecProductAssignmentOC(specprodassignment, logger));

                // Generate Variants
                var variantGroups = variantPlaceholders.GroupBy(v => v.ProductID);
                foreach (IList<VariantPlaceholder> variantGroup in variantGroups)
                {
                    // Allow the system to generate variants based on selection specs
                    await GenerateOCVariants(variantGroup[0].ProductID, logger);

                    //Modify the generated specs to use custom variant id's
                    await Throttler.RunAsync(variantGroup, 500, 1, variant => UpdateProductVariantsOC(variant, logger));
                }
            }
            logger.LogInformation($"Process BrandwearProducts Complete.");
        }
    }
}
