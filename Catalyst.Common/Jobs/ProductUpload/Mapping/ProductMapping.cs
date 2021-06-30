using Catalyst.Common.ProductUpload.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalyst.Common.ProductUpload.Mapping
{
    class ProductMapping
    {
        private static string CleanOrderCloudID(string val)
        {
            var ocID = Regex.Replace(val, "[:!@#$%^&*()}{|\":?><\\[\\]\\;'/.,~]", "").Replace(" ", "");
            return ocID;
        }
        private static List<PriceBreak> MapPriceBreaks(SampleProduct product)
        {
            if (product == null) return null;
            var list = new List<PriceBreak>();
            list.Add(new PriceBreak
            {
                Price = decimal.Parse(!String.IsNullOrEmpty(product.variants[0].price) ? product.variants[0].price : "0"),
                Quantity = 1
            });
            return list;
        }
        public static PriceSchedule MapOCPriceSchedule(SampleProduct product)
        {
            var priceSchedule = new PriceSchedule()
            {
                ApplyShipping = true,
                ApplyTax = true,
                ID = CleanOrderCloudID(product.id),
                Name = product.title,
                RestrictedQuantity = true,
                UseCumulativeQuantity = false,
                PriceBreaks = MapPriceBreaks(product)

            };
            return priceSchedule;
        }
        public static Product MapOCProduct(SampleProduct product)
        {
            var p = new Product
            {
                Active = true,
                AutoForward = false,
                Name = product.title,
                ID = CleanOrderCloudID(product.id),
                Description = product.body_html,
                DefaultSupplierID = null,
                DefaultPriceScheduleID = CleanOrderCloudID(product.id),
                QuantityMultiplier = 1
            };
            return p;
        }
        public static Spec MapOCProductSpec(SampleOption option)
        {
            var spec = new Spec()
            {
                ID = $"{CleanOrderCloudID(option.product_id)}-{CleanOrderCloudID(option.name)}",
                AllowOpenText = false,
                DefinesVariant = true,
                Required = true,
                Name = option.name
            };

            return spec;
        }
        public static SpecOption MapOCProductSpecOption(string productID, string specName, string val)
        {
            var o = new SpecOption()
            {
                ID = $"{CleanOrderCloudID(productID)}-{CleanOrderCloudID(specName)}-{CleanOrderCloudID(val)}",
                IsOpenText = false,
                Value = val,
                PriceMarkupType = PriceMarkupType.NoMarkup,
                PriceMarkup = 0,
                xp = {
                    Description = val,
                    SpecID = $"{CleanOrderCloudID(productID)}-{CleanOrderCloudID(specName)}"
                }
            };
            return o;
        }
        public static VariantPlaceholder MapOCProductVariant(SampleProduct product, SampleOption option, string val)
        {
            string variantSKU = "";
            foreach (SampleVariant variant in product.variants)
            {
                if (variant.option1 == val)
                {
                    variantSKU = variant.sku;
                }
            }
            var variantPlaceholder = new VariantPlaceholder()
            {
                ProductID = CleanOrderCloudID(option.product_id),
                SpecID = $"{CleanOrderCloudID(option.product_id)}-{CleanOrderCloudID(option.name)}",
                SpecOptionID = $"{CleanOrderCloudID(option.product_id)}-{CleanOrderCloudID(option.name)}-{CleanOrderCloudID(val)}",
                VariantSKU = variantSKU
            };
            return variantPlaceholder;
        }
        public static SpecProductAssignment MapOCProductSpecAssignment(string productID, string specName)
        {
            var spa = new SpecProductAssignment()
            {
                SpecID = $"{CleanOrderCloudID(productID)}-{CleanOrderCloudID(specName)}",
                ProductID = CleanOrderCloudID(productID)
            };
            return spa;
        }
    }
}
