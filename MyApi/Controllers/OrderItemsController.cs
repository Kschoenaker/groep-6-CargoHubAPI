using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v2/orderitems")]
public class OrderItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrderItemsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> getOrderItems()
    {
        var orderItems = await _context.OrderItems.ToListAsync();

        return Ok(orderItems);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderItem(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);

        await _context.SaveChangesAsync();

        return Ok(orderItem);
    }
}