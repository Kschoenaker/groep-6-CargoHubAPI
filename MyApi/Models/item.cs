//Artikelregels beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;


namespace MyApi.Models;

public class Item
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Model_Number { get; set; }
    public int? Commodity_Code { get; set; }
    public decimal? Unit_Weight { get; set; }
    public int? Item_Line_Id { get; set; }
    public int? Item_Group_Id { get; set; }
    public int? Item_Type_Id { get; set; }
    public int? Min_Purchase_Qty { get; set; }
    public int? Case_Size { get; set; }
    public string? Packaging_Type { get; set; }
    public int? Order_Multiple { get; set; }
    public int? Supplier_Id { get; set; }
    public string? Supplier_Sku { get; set; }
    public DateTime? Created_At { get; set; }
    public DateTime? Updated_At { get; set; }
}
