using System.ComponentModel.DataAnnotations;
namespace MyApi.Models;

public class OrderItem
{
    public int id { get; set; }
    public int order_id { get; set; }
    public int item_id { get; set; }
        [Range(1, int.MaxValue)]
    public int amount { get; set; }
    public decimal unit_price { get; set; }
}
