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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderItem(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
        {
            return NotFound();
        }

        return Ok(orderItem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderItem(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);

        await _context.SaveChangesAsync();

        return Ok(orderItem);
    }

    // werkt de gegevens van een bestaande order item bij, afhankelijk van het id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem)
    {
        // Zoek de bestaande order item op met het ID uit de URL
        var existingOrderItem = await _context.OrderItems.FindAsync(id);

        if (existingOrderItem == null)
        {
            return NotFound();
        }
        // Neem de nieuwe waarden uit de request over
        existingOrderItem.order_id = orderItem.order_id;
        existingOrderItem.item_id = orderItem.item_id;
        existingOrderItem.amount = orderItem.amount;
        existingOrderItem.unit_price = orderItem.unit_price;

        await _context.SaveChangesAsync();

        return Ok(existingOrderItem);
    }

    //verwijdert de order item met dit ID 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItems(int id)
    {
        var existingOrderItem = await _context.OrderItems.FindAsync(id);

        if (existingOrderItem == null)
        {
            return NotFound();
        }

        _context.OrderItems.Remove(existingOrderItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}