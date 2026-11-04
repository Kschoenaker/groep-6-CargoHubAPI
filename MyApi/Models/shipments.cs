namespace MyApi.Models;

public class Shipments
{
    public int id { get; set; }
    public string reference { get; set; }
    public int order_id { get; set; }
    public DateTime shipment_date { get; set; }
    public string shipment_type { get; set; }
    public string shipment_status { get; set; }
    public string carrier_name { get; set; }
    public string shipping_method { get; set; }
    public string payment_type { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
