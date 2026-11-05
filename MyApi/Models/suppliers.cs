namespace MyApi.Models;

public class Supplier
{
    public int id { get; set; }
    public string province { get; set; }
    public string country { get; set; }
    public string contact_name { get; set; } 
    public string phone_number { get; set; }
    public string reference { get; set; } 
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
