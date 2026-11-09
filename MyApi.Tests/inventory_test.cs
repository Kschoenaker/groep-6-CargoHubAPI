using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit; //doe dit erbij 

namespace MyApi.Tests;

public class InventoriesControllerTests
{
    private static AppDbContext CreateContext() //temporary database
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Inventory CreateInventory() //mock data
    {
        return new Inventory
        {
            item_id = 1,
            location_id = 1,
            quantity_on_hand = 10,
            quantity_expected = 5,
            quantity_ordered = 3,
            quantity_allocated = 2,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    //integratie test
    [Fact] //checkt if alle inventories worden returned, check the returned list, test data retrieval
    public async Task GetInventories_ReturnsAllInventories() //unit test
    {
        using var context = CreateContext();
        context.Inventories.Add(CreateInventory());
        await context.SaveChangesAsync();

        var controller = new InventoriesController(context);

        var result = await controller.GetInventories();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var inventories = Assert.IsType<List<Inventory>>(okResult.Value);
        Assert.Single(inventories);
    }

//unit test?
[Fact] // test of de method HTTP 2000 returned, checkt alleen of de method werkt
public async Task GetInventories_ReturnsOk()
{
    using var context = CreateContext();
    var controller = new InventoriesController(context);

    var result = await controller.GetInventories();

    Assert.IsType<OkObjectResult>(result);
}


    // integration test
    [Fact] // test if a new inventory can be created
    public async Task CreateInventory_Test()
    {
        using var context = CreateContext();

        var inventory = CreateInventory();

        var controller = new InventoriesController(context);

        var result = await controller.CreateInventory(inventory);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var createdInventory = Assert.IsType<Inventory>(okResult.Value);

        Assert.Equal(1, createdInventory.item_id);
        Assert.Equal(1, createdInventory.location_id);

        Assert.Single(context.Inventories);
    }



[Fact] // test if an inventory can be updated
public async Task UpdateInventory_ReturnsOk()
{
    using var context = CreateContext();

    context.Inventories.Add(CreateInventory());
    await context.SaveChangesAsync();

    var inventory = CreateInventory();
    inventory.quantity_on_hand = 20;

    var controller = new InventoriesController(context);

    var result = await controller.UpdateInventory(1, 1, inventory);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var updatedInventory = Assert.IsType<Inventory>(okResult.Value);

    Assert.Equal(20, updatedInventory.quantity_on_hand);
}


// integration test
[Fact] // test if an inventory can be deleted
public async Task DeleteInventory_ReturnsOk()
{
    using var context = CreateContext();

    var inventory = CreateInventory();

    context.Inventories.Add(inventory);
    await context.SaveChangesAsync();

    var controller = new InventoriesController(context);

    var result = await controller.DeleteInventory(1, 1);

    Assert.IsType<OkResult>(result);

    Assert.Empty(context.Inventories);
}

}