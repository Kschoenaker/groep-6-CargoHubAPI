namespace MyApi.Models;

public class Location
{
    public int id { get; set;}
    public int warehouse_id { get; set;}
    public string code { get; set;}
    public string name { get; set;}
    public DateTime created_at { get; set;}
    public DateTime updated_at { get; set;}
}