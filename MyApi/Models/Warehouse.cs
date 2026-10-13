
namespace MyApi.Models;

public class Warehouse
{
    public int id { get; set; }

    public string code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public string Province { get; set; } = "";
    public string Country { get; set; } = "";
    public string ContactName { get; set; } = "";
    public string ContactPhone { get; set; } = "";
    public string ContactEmail { get; set; } = "";

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
