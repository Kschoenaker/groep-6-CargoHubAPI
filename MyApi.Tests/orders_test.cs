using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit;

namespace MyApi.Tests;

public class OrdersControllerTests
{
    private static AppDbContext CreateContext() // temporary database
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Order CreateOrder() // mock data
    {
        return new Order
        {
            id = 1,
            client_id = 1,
            ship_to_client_id = 1,
            bill_to_client_id = 1,
            warehouse_id = 1,
            order_date = "2026-11-06",
            request_date = "2026-11-10",
            reference = "ORD-001",
            customer_po_number = "PO-001",
            order_status = "Pending",
            shipping_notes = "Handle with care",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    // integration test
    [Fact]
    public async Task GetOrders_ReturnsAllOrders()
    {
        using var context = CreateContext();

        context.Orders.Add(CreateOrder());
        await context.SaveChangesAsync();

        var controller = new OrdersController(context);

        var result = await controller.GetOrders();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var orders = Assert.IsType<List<Order>>(okResult.Value);

        Assert.Single(orders);
    }

    //unit test
    [Fact]
    public async Task GetOrders_ReturnsOk()
    {
        using var context = CreateContext();

        var controller = new OrdersController(context);

        var result = await controller.GetOrders();

        Assert.IsType<OkObjectResult>(result);
    }



[Fact] // test if a new order can be created
public async Task CreateOrder_Test()
{
    using var context = CreateContext();

    var order = CreateOrder();

    var controller = new OrdersController(context);

    var result = await controller.CreateOrder(order);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var createdOrder = Assert.IsType<Order>(okResult.Value);

    Assert.Equal(1, createdOrder.id);
    Assert.Equal("ORD-001", createdOrder.reference);

    Assert.Single(context.Orders);
}


[Fact] // test if an order can be updated
public async Task UpdateOrder_ReturnsOk()
{
    using var context = CreateContext();

    context.Orders.Add(CreateOrder());
    await context.SaveChangesAsync();

    var order = CreateOrder();
    order.order_status = "Shipped";

    var controller = new OrdersController(context);

    var result = await controller.UpdateOrder(1, order);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var updatedOrder = Assert.IsType<Order>(okResult.Value);

    Assert.Equal("Shipped", updatedOrder.order_status);
}


[Fact] // test if an order can be deleted
public async Task DeleteOrder_ReturnsOk()
{
    using var context = CreateContext();

    context.Orders.Add(CreateOrder());
    await context.SaveChangesAsync();

    var controller = new OrdersController(context);

    var result = await controller.DeleteOrder(1);

    Assert.IsType<OkResult>(result);

    Assert.Empty(context.Orders);
}

}
