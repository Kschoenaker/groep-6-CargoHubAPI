using System.ComponentModel.DataAnnotations;

namespace MyApi.Models;

public class ShipmentItems
{
    public int id { get; set; }
    public int shipment_id { get; set; }
    public int item_id { get; set; }
    [Range(1, int.MaxValue)]
    public int amount { get; set; }
}
