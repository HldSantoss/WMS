using System;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace Domain.Entities.Intelipost
{
    public class ContentDeclaration
    {
        [JsonPropertyName("content_declaration_number")]
        public string Content_declaration_number { get; set; }
        [JsonPropertyName("content_declaration_total_value")]
        public string Content_declaration_total_value { get; set; }
        [JsonPropertyName("content_declaration_date")]
        public string Content_declaration_date { get; set; }
    }

    public class EndCustomer
    {
        [JsonPropertyName("first_name")]
        public string First_name { get; set; }
        [JsonPropertyName("last_name")]
        public string Last_name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
        [JsonPropertyName("cellphone")]
        public string Cellphone { get; set; }
        [JsonPropertyName("is_company")]
        public bool Is_company { get; set; }
        [JsonPropertyName("federal_tax_payer_id")]
        public string Federal_tax_payer_id { get; set; }
        [JsonPropertyName("shipping_country")]
        public string Shipping_country { get; set; }
        [JsonPropertyName("shipping_state")]
        public string Shipping_state { get; set; }
        [JsonPropertyName("shipping_city")]
        public string Shipping_city { get; set; }
        [JsonPropertyName("shipping_address")]
        public string Shipping_address { get; set; }
        [JsonPropertyName("shipping_number")]
        public string Shipping_number { get; set; }
        [JsonPropertyName("shipping_quarter")]
        public string Shipping_quarter { get; set; }
        [JsonPropertyName("shipping_additional")]
        public string Shipping_additional { get; set; }
        [JsonPropertyName("shipping_zip_code")]
        public string Shipping_zip_code { get; set; }
        [JsonPropertyName("shipping_reference")]
        public string Shipping_reference { get; set; }
        [JsonPropertyName("opt_in")]
        public bool Opt_in { get; set; }
        [JsonPropertyName("notify")]
        public NotifyClient Notify { get; set; }
    }

    public class ExternalOrderNumbers
    {
        [JsonPropertyName("marketplace")]
        public string Marketplace { get; set; }
        [JsonPropertyName("sales")]
        public string Sales { get; set; }
        [JsonPropertyName("platforma")]
        public string Plataforma { get; set; }
        [JsonPropertyName("erp")]
        public string Erp { get; set; }
    }

    public class NotifyClient
    {
        [JsonPropertyName("whatsapp")]
        public bool Whatsapp { get; set; }
    }

    public class ProductIP
    {
        [JsonPropertyName("image_url")]
        public string Image_url { get; set; }
        [JsonPropertyName("price")]
        public double Price { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("sku")]
        public string Sku { get; set; }
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public class OrderIntelipost
    {
        [JsonPropertyName("order_number")]
        public string Order_number { get; set; }
        [JsonPropertyName("parent_shipment_order_number")]
        public string Parent_shipment_order_number { get; set; }
        [JsonPropertyName("sales_order_number")]
        public string Sales_order_number { get; set; }
        [JsonPropertyName("delivery_method_id")]
        public int Delivery_method_id { get; set; }
        [JsonPropertyName("delivery_method_external_id")]
        public string Delivery_method_external_id { get; set; }
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
        [JsonPropertyName("estimated_delivery_date")]
        public DateTime Estimated_delivery_date { get; set; }
        [JsonPropertyName("scheduled")]
        public bool Scheduled { get; set; }
        [JsonPropertyName("sales_channel")]
        public string Sales_channel { get; set; }
        [JsonPropertyName("shipment_order_type")]
        public string Shipment_order_type { get; set; }
        [JsonPropertyName("customer_shipping_costs")]
        public int Customer_shipping_costs { get; set; }
        [JsonPropertyName("provider_shipping_costs")]
        public double Provider_shipping_costs { get; set; }
        [JsonPropertyName("origin_zip_code")]
        public string Origin_zip_code { get; set; }
        [JsonPropertyName("origin_warehouse_code")]
        public string Origin_warehouse_code { get; set; }
        [JsonPropertyName("end_customer")]
        public EndCustomer End_customer { get; set; }
        [JsonPropertyName("shipment_order_volume_array")]
        public List<ShipmentOrderVolumeArray> Shipment_order_volume_array { get; set; }
        [JsonPropertyName("content_declaration")]
        public ContentDeclaration Content_declaration { get; set; }
    }

    public class ShipmentOrderVolumeArray
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("tracking_code")]
        public string Tracking_code { get; set; }
        [JsonPropertyName("shipment_order_volume_number")]
        public int Shipment_order_volume_number { get; set; }
        [JsonPropertyName("volume_type_code")]
        public string Volume_type_code { get; set; }
        [JsonPropertyName("client_pre_shipment_list")]
        public string Client_pre_shipment_list { get; set; }
        [JsonPropertyName("weight")]
        public int Weight { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("lenght")]
        public int Length { get; set; }
        [JsonPropertyName("products_quantity")]
        public int Products_quantity { get; set; }
        [JsonPropertyName("products_nature")]
        public string Products_nature { get; set; }
        [JsonPropertyName("products")]
        public List<ProductIP> Products { get; set; }
        [JsonPropertyName("shipment_order_volume_invoice")]
        public ShipmentOrderVolumeInvoice Shipment_order_volume_invoice { get; set; }
    }

    public class ShipmentOrderVolumeInvoice
    {
        [JsonPropertyName("invoice_series")]
        public string Invoice_series { get; set; }
        [JsonPropertyName("invoice_number")]
        public string Invoice_number { get; set; }
        [JsonPropertyName("invoice_key")]
        public string Invoice_key { get; set; }
        [JsonPropertyName("invoice_date")]
        public DateTime Invoice_date { get; set; }
        [JsonPropertyName("invoice_total_value")]
        public double Invoice_total_value { get; set; }
        [JsonPropertyName("invoice_products_value")]
        public double Invoice_products_value { get; set; }
        [JsonPropertyName("invoice_cfop")]
        public string Invoice_cfop { get; set; }
    }

    public class ReturnOrder
    {
         public Content content { get; set; }
    }

    public class Content
    {
        public string tracking_url { get; set; }
    }

    public class TrackingData
    {
        public string order_number { get; set; }
        public List<TrackingDataArray> tracking_data_array { get; set; }
    }

    public class TrackingDataArray
    {
        public int shipment_order_volume_number { get; set; }
        public string tracking_code { get; set; }
    }
}

