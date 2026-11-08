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
    public async Task<IActionResult> GetShipments()
    {
        var shipment = await _context.Shipments.ToListAsync();

        return Ok(shipment);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipments(int id)
    {
        var shipment = await _context.Shipments.FindAsync(id);

        if (shipment == null)
        {
            return NotFound();
        }

        return Ok(shipment);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShipments(Shipments shipment)
    {
        _context.Shipments.Add(shipment);

        await _context.SaveChangesAsync();

        return Ok(shipment);
    }

    // werkt de gegevens van een bestaande shipment bij, afhankelijk van het id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShipments(int id, Shipments shipment)
    {
        // Zoek de bestaande shipment op met het ID uit de URL
        var existingShipment = await _context.Shipments.FindAsync(id);

        if (existingShipment == null)
        {
            return NotFound();
        }
        // Neem de nieuwe waarden uit de request over
        existingShipment.reference = shipment.reference;
        existingShipment.order_id = shipment.order_id;
        existingShipment.shipment_date = shipment.shipment_date;
        existingShipment.shipment_type = shipment.shipment_type;
        existingShipment.shipment_status = shipment.shipment_status;
        existingShipment.carrier_name = shipment.carrier_name;
        existingShipment.shipping_method = shipment.shipping_method;
        existingShipment.payment_type = shipment.payment_type;
        existingShipment.created_at = shipment.created_at;
        existingShipment.updated_at = shipment.updated_at;

        await _context.SaveChangesAsync();

        return Ok(existingShipment);
    }

    //verwijdert de shipment met dit ID 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShipments(int id)
    {
        var existingShipment = await _context.Shipments.FindAsync(id);

        if (existingShipment == null)
        {
            return NotFound();
        }

        _context.Shipments.Remove(existingShipment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

//CRUD : Create, Read, Update, Delete