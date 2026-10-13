namespace MyApi.Models;

public class ShipmentItems
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public int ItemId { get; set; }
    public int Amount { get; set; }
}
