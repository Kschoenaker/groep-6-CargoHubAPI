//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v2/shipment-items")]
public class ShipmentItemsController : ControllerBase
{

    private readonly AppDbContext _context;

    public ShipmentItemsController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> getShipmentItems()
    {
        var shipment_items = await _context.ShipmentItems.ToListAsync();

        return Ok(shipment_items);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateShipmentItem(ShipmentItems shipmentItem)
    {
        _context.ShipmentItems.Add(shipmentItem);

        await _context.SaveChangesAsync();

        return Ok(shipmentItem);
    }

}

//CRUD : Create, Read, Update, Delete