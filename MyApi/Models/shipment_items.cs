namespace MyApi.Models;

public class ShipmentItems
{
    public int id { get; set; }
    public int shipment_id { get; set; }
    public int item_id { get; set; }
    public int amount { get; set; }
}
