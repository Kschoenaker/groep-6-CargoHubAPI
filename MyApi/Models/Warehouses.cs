namespace MyApi.Models;

public class Warehouse
{
    public int Id { get; set;}
    public string Name { get; set;}
    public int Code { get; set;}
    public string Address {get; set;}
    public string City {get; set;}
    public int Zip_Code {get; set;}
    public string Province {get; set;}
    public string Country {get; set;}
    public string Contact_Name { get; set;}
    public string Contact_Phone { get; set;}
    public string Contact_Email { get; set;}
    public string Created_At { get; set;}
    public string Updated_At { get; set;}
}