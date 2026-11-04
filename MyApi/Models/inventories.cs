namespace MyApi.Models;

public class Inventory
{
    public int item_id { get; set; }
    public int location_id { get; set; }
    public int quantity_on_hand { get; set; }
    public int quantity_expected { get; set; }
    public int quantity_ordered { get; set; }
    public int quantity_allocated { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
