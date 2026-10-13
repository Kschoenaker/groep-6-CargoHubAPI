//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v1/shipment-items")]
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

}

//CRUD : Create, Read, Update, Delete