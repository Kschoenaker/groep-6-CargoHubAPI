using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit; //doe dit erbij 

namespace MyApi.Tests;

public class OrderItemsControllerTests
{
    private static AppDbContext CreateContext() //temporary database
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static OrderItem CreateOrderItem()
    {
        return new OrderItem
        {
            id = 1,
            order_id = 1,
            item_id = 1,
            amount = 10,
            unit_price = 33.33m
        };
    }

    [Fact]
    public async Task GetOrderItems_ReturnsAllOrderItems() //integration test
    {
        using var context = CreateContext();
        context.OrderItems.Add(CreateOrderItem());
        await context.SaveChangesAsync();

        var controller = new OrderItemsController(context);

        var result = await controller.getOrderItems();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var orderItems = Assert.IsType<List<OrderItem>>(okResult.Value);
        Assert.Single(orderItems);
    }

    [Fact]
    public async Task GetOrderItem_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new OrderItemsController(context);

        var result = await controller.GetOrderItem(999); // Assuming 999 is an unknown ID

        Assert.IsType<NotFoundResult>(result);
    }
}