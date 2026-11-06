namespace MyApi.Models;

public class Transfer
{
    public int id { get; set;}
    public string reference { get; set;}
    public int from_location_id { get; set;}
    public int to_location_id { get; set;}
    public string transfer_status { get; set;}
    public DateTime created_at { get; set;}
    public DateTime updated_at { get; set;}
}