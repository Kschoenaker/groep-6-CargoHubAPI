//Verzendingen beheren

//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v2/shipments")]
public class ShipmentsController : ControllerBase
{

    private readonly AppDbContext _context;

    public ShipmentsController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> getShipments()
    {
        var shipment = await _context.Shipments.ToListAsync();

        return Ok(shipment);
    }

}

//CRUD : Create, Read, Update, Delete