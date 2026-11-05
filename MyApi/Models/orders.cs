namespace MyApi.Models;

public class Order
{
    public int id { get; set; }
    public int client_id { get; set; }
    public int ship_to_client_id { get; set; }
    public int bill_to_client_id { get; set; }
    public int warehouse_id { get; set; }
    public string order_date { get; set; }
    public string request_date { get; set; }
    public string reference { get; set; }
    public string customer_po_number { get; set; }
    public string order_status { get; set; }
    public string shipping_notes { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}