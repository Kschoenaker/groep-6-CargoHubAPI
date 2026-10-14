//Verzendingen beheren

//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

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

    [HttpPost]
    public async Task<IActionResult> CreateShipments(Shipments shipment)
    {
        _context.Shipments.Add(shipment);

        await _context.SaveChangesAsync();

        return Ok(shipment);
    }
}

//CRUD : Create, Read, Update, Delete