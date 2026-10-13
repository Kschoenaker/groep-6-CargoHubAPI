namespace MyApi.Models;

public class Warehouse
{
    public int id { get; set;}
    public int code { get; set;}
    public string name { get; set;}
    public string address {get; set;}
    public string city {get; set;}
    public int zip_code {get; set;}
    public string province {get; set;}
    public string country {get; set;}
    public string contact_name { get; set;}
    public string contact_phone { get; set;}
    public string contact_email { get; set;}
    public string created_at { get; set;}
    public string updated_at { get; set;}
}