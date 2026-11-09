using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit;


namespace MyApi.Tests;

public class ItemControllerTests
{
    // TODO - Minimaal nog testen:
    // - GetItemById geeft BadRequest bij een ongeldig ID en NotFound bij een onbekend ID.
    // - Create geeft BadRequest bij null.
    // - UpdateItem geeft BadRequest bij een ongeldig of afwijkend ID en NotFound bij een onbekend item.
    // - DeleteItem geeft BadRequest bij een ongeldig ID en NotFound bij een onbekend item.

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Item CreateItem(int id = 1)
    {
        return new Item
        {
            Id = id,
            Code = $"ITM-{id:000000}",
            Description = $"Test item {id}",
            Barcode = "123456789",
            Model_Number = "MODEL-1",
            Commodity_Code = 123,
            Unit_Weight = 1.25m,
            Item_Line_Id = 2,
            Item_Group_Id = 3,
            Item_Type_Id = 4,
            Min_Purchase_Qty = 5,
            Case_Size = 6,
            Packaging_Type = "Case",
            Order_Multiple = 1,
            Supplier_Id = 7,
            Supplier_Sku = "SKU-1",
            Created_At = DateTime.UtcNow,
            Updated_At = DateTime.UtcNow
        };
    }

    [Fact]
    public void ItemMapsToDatabaseTableAndColumns()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(Item));
        var table = StoreObjectIdentifier.Table("items", null);

        Assert.NotNull(entity);
        Assert.Equal("items", entity.GetTableName());
        Assert.Equal("id", entity.FindProperty(nameof(Item.Id))!.GetColumnName(table));
        Assert.Equal("item_type_id", entity.FindProperty(nameof(Item.Item_Type_Id))!.GetColumnName(table));
        Assert.Equal("created_at", entity.FindProperty(nameof(Item.Created_At))!.GetColumnName(table));
    }

    [Fact]
    public async Task GetItems_ReturnsAllItems()
    {
        using var context = CreateContext();
        context.item.AddRange(CreateItem(), CreateItem(2));
        await context.SaveChangesAsync();

        var controller = new ItemController(context);

        var result = await controller.GetItems();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsType<List<Item>>(okResult.Value);
        Assert.Equal(2, items.Count);
    }

    // -----------------------------------------------------------------------------------------------------------------------------
    [Fact]
    public async Task GetItemById_ReturnsItem()
    {
        using var context = CreateContext();
        context.item.Add(CreateItem());
        await context.SaveChangesAsync();

        var controller = new ItemController(context);

        var result = await controller.GetItemById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, Assert.IsType<Item>(okResult.Value).Id);
    }

    [Fact]
    public async Task Create_ReturnsCreatedItem()
    {
        using var context = CreateContext();
        var item = CreateItem();
        var controller = new ItemController(context);

        var result = await controller.Create(item);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ItemController.GetItemById), createdResult.ActionName);
        Assert.Equal(item.Id, (await context.item.SingleAsync()).Id);
    }

    [Fact]
    public async Task UpdateItem_UpdatesAllItemFields()
    {
        using var context = CreateContext();
        context.item.Add(CreateItem());
        await context.SaveChangesAsync();
        var updatedItem = new Item
        {
            Id = 1,
            Code = "ITM-UPDATED",
            Description = "Updated description",
            Barcode = "987654321",
            Model_Number = "MODEL-UPDATED",
            Commodity_Code = 456,
            Unit_Weight = 2.5m,
            Item_Line_Id = 12,
            Item_Group_Id = 13,
            Item_Type_Id = 14,
            Min_Purchase_Qty = 15,
            Case_Size = 16,
            Packaging_Type = "Pallet",
            Order_Multiple = 2,
            Supplier_Id = 17,
            Supplier_Sku = "SKU-UPDATED",
            Created_At = DateTime.UtcNow.AddDays(-1),
            Updated_At = DateTime.UtcNow.AddDays(-1)
        };
        var controller = new ItemController(context);

        var result = await controller.UpdateItem(1, updatedItem);

        Assert.IsType<NoContentResult>(result);
        var savedItem = await context.item.AsNoTracking().SingleAsync();
        Assert.Equal(updatedItem.Code, savedItem.Code);
        Assert.Equal(updatedItem.Description, savedItem.Description);
        Assert.Equal(updatedItem.Barcode, savedItem.Barcode);
        Assert.Equal(updatedItem.Model_Number, savedItem.Model_Number);
        Assert.Equal(updatedItem.Commodity_Code, savedItem.Commodity_Code);
        Assert.Equal(updatedItem.Unit_Weight, savedItem.Unit_Weight);
        Assert.Equal(updatedItem.Item_Line_Id, savedItem.Item_Line_Id);
        Assert.Equal(updatedItem.Item_Group_Id, savedItem.Item_Group_Id);
        Assert.Equal(updatedItem.Item_Type_Id, savedItem.Item_Type_Id);
        Assert.Equal(updatedItem.Min_Purchase_Qty, savedItem.Min_Purchase_Qty);
        Assert.Equal(updatedItem.Case_Size, savedItem.Case_Size);
        Assert.Equal(updatedItem.Packaging_Type, savedItem.Packaging_Type);
        Assert.Equal(updatedItem.Order_Multiple, savedItem.Order_Multiple);
        Assert.Equal(updatedItem.Supplier_Id, savedItem.Supplier_Id);
        Assert.Equal(updatedItem.Supplier_Sku, savedItem.Supplier_Sku);
        Assert.NotEqual(updatedItem.Updated_At, savedItem.Updated_At);
    }

    [Fact]
    public async Task DeleteItem_RemovesItem()
    {
        using var context = CreateContext();
        context.item.Add(CreateItem());
        await context.SaveChangesAsync();
        var controller = new ItemController(context);

        var result = await controller.DeleteItem(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(await context.item.ToListAsync());
    }
}
