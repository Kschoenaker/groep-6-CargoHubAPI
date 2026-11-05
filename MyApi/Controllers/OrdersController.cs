//Bestellingen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.Orders.ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.id == id);

    if (order == null)
    {
        return NotFound();
    }

    return Ok(order);
}

    // post
    [HttpPost]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        return Ok(order);
    }

    // put
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order order)
    {
        var existingOrder = await _context.Orders.FirstOrDefaultAsync(x => x.id == id);

        if (existingOrder == null)
        {
            return NotFound();
        }

        existingOrder.client_id = order.client_id;
        existingOrder.ship_to_client_id = order.ship_to_client_id;
        existingOrder.bill_to_client_id = order.bill_to_client_id;
        existingOrder.warehouse_id = order.warehouse_id;
        existingOrder.order_date = order.order_date;
        existingOrder.request_date = order.request_date;
        existingOrder.reference = order.reference;
        existingOrder.customer_po_number = order.customer_po_number;
        existingOrder.order_status = order.order_status;
        existingOrder.shipping_notes = order.shipping_notes;
        existingOrder.updated_at = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingOrder);
    }

    // delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
          var order = await _context.Orders.FirstOrDefaultAsync(x => x.id == id);

        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok();
    }


}