namespace MyApi.Models;

public class Shipments
{
    public int Id { get; set; }
    public string Reference { get; set; }
    public int OrderId { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string ShipmentType { get; set; }
    public string ShipmentStatus { get; set; }
    public string CarrierName { get; set; }
    public string ShippingMethod { get; set; }
    public string PaymentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
