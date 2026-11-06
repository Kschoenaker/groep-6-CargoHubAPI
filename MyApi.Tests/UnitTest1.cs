using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit;

namespace MyApi.Tests;

public class InventoriesControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Inventory CreateInventory()
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

    [Fact]
    public async Task GetInventories_ReturnsAllInventories()
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
}