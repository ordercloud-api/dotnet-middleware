using System;
using System.Collections.Generic;
using System.Text;

namespace Catalyst.Common.ProductUpload.Models
{
    public class VariantPlaceholder
    {
        public string ProductID { get; set; }
        public string SpecID { get; set; }
        public string SpecOptionID { get; set; }
        public string VariantSKU { get; set; }

    }
    public class SampleProductList
    {
        public List<SampleProduct> products { get; set; }
    }

    public class SampleProduct
    {
        public string id { get; set; }
        public string title { get; set; }
        public string body_html { get; set; }
        public string vendor { get; set; }
        public string product_type { get; set; }
        public DateTime created_at { get; set; }
        public string handle { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime published_at { get; set; }
        public object template_suffix { get; set; }
        public string published_scope { get; set; }
        public string tags { get; set; }
        public string admin_graphql_api_id { get; set; }
        public List<SampleVariant> variants { get; set; }
        public List<SampleOption> options { get; set; }
        public List<VariantImages> images { get; set; }
        public ProductImage image { get; set; }
    }

    public class ProductImage
    {
        public string id { get; set; }
        public int product_id { get; set; }
        public int position { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object alt { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string src { get; set; }
        public List<string> variant_ids { get; set; }
        public string admin_graphql_api_id { get; set; }
    }

    public class SampleVariant
    {
        public string id { get; set; }
        public int product_id { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string sku { get; set; }
        public int position { get; set; }
        public string inventory_policy { get; set; }
        public object compare_at_price { get; set; }
        public string fulfillment_service { get; set; }
        public string inventory_management { get; set; }
        public string option1 { get; set; }
        public object option2 { get; set; }
        public object option3 { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool taxable { get; set; }
        public string barcode { get; set; }
        public int grams { get; set; }
        public int? image_id { get; set; }
        public float weight { get; set; }
        public string weight_unit { get; set; }
        public int inventory_item_id { get; set; }
        public int inventory_quantity { get; set; }
        public int old_inventory_quantity { get; set; }
        public List<Presentment_Prices> presentment_prices { get; set; }
        public bool requires_shipping { get; set; }
        public string admin_graphql_api_id { get; set; }
    }

    public class Presentment_Prices
    {
        public SamplePrice price { get; set; }
        public object compare_at_price { get; set; }
    }

    public class SamplePrice
    {
        public string amount { get; set; }
        public string currency_code { get; set; }
    }

    public class SampleOption
    {
        public string id { get; set; }
        public string product_id { get; set; }
        public string name { get; set; }
        public int position { get; set; }
        public List<string> values { get; set; }
    }

    public class VariantImages
    {
        public string id { get; set; }
        public int product_id { get; set; }
        public int position { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object alt { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string src { get; set; }
        public List<string> variant_ids { get; set; }
        public string admin_graphql_api_id { get; set; }
    }


}
