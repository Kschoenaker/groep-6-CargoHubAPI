using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit; //doe dit erbij 

namespace MyApi.Tests;

public class ShipmentControllerTests
{
    private static AppDbContext CreateContext() //temporary database
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Shipments CreateShipment()
    {
        return new Shipments
        {
            id = 1,
            reference = "TEST-001",
            order_id = 10,
            shipment_date = DateTime.UtcNow,
            shipment_type = "Outbound",
            shipment_status = "Pending",
            carrier_name = "Test Carrier",
            shipping_method = "Standard",
            payment_type = "Prepaid",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetShipments_ReturnsAllShipments() //integration test
    {
        using var context = CreateContext();
        context.Shipments.Add(CreateShipment());
        await context.SaveChangesAsync();

        var controller = new ShipmentsController(context);

        var result = await controller.GetShipments();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var shipments = Assert.IsType<List<Shipments>>(okResult.Value);
        Assert.Single(shipments);
    }

    [Fact]
    public async Task GetShipment_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new ShipmentsController(context);

        var result = await controller.GetShipments(id: 999);

        Assert.IsType<NotFoundResult>(result);
    }
}