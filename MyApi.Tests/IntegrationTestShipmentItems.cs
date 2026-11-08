using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit; //doe dit erbij 

namespace MyApi.Tests;

public class ShipmentItemsControllerTests
{
    private static AppDbContext CreateContext() //temporary database
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ShipmentItems CreateShipmentItem()
    {
        return new ShipmentItems
        {
            id = 1,
            shipment_id = 1,
            item_id = 1,
            amount = 3
        };
    }

    [Fact]
    public async Task GetShipmentItems_ReturnsAllShipmentItems() //integration test
    {
        using var context = CreateContext();
        context.ShipmentItems.Add(CreateShipmentItem());
        await context.SaveChangesAsync();

        var controller = new ShipmentItemsController(context);

        var result = await controller.getShipmentItems();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var shipmentItems = Assert.IsType<List<ShipmentItems>>(okResult.Value);
        Assert.Single(shipmentItems);
    }

    [Fact]
    public async Task GetShipmentItem_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new ShipmentItemsController(context);

        var result = await controller.GetShipmentItem(999); // Assuming 999 is an unknown ID

        Assert.IsType<NotFoundResult>(result);
    }
}