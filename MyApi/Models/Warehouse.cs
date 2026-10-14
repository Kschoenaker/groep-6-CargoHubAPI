namespace MyApi.Models;

public class Warehouse
{
    public int id { get; set;}
    public string code { get; set;}
    public string name { get; set;}
    public string address {get; set;}
    public string city {get; set;}
    public string zip_code {get; set;}
    public string province {get; set;}
    public string country {get; set;}
    public string contact_name { get; set;}
    public string contact_phone { get; set;}
    public string contact_email { get; set;}
    public DateTime created_at { get; set;}
    public DateTime updated_at { get; set;}
}