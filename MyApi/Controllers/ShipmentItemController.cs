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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipmentItem(int id)
    {
        var shipmentItem = await _context.ShipmentItems.FindAsync(id);

        if (shipmentItem == null)
        {
            return NotFound();
        }

        return Ok(shipmentItem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShipmentItem(ShipmentItems shipmentItem)
    {
        _context.ShipmentItems.Add(shipmentItem);

        await _context.SaveChangesAsync();

        return Ok(shipmentItem);
    }

        // werkt de gegevens van een bestaande shipment item bij, afhankelijk van het id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShipmentItem(int id, ShipmentItems shipmentItem)
    {
        // Zoek de bestaande shipment item op met het ID uit de URL
        var existingShipmentItem = await _context.ShipmentItems.FindAsync(id);

        if (existingShipmentItem == null)
        {
            return NotFound();
        }
        // Neem de nieuwe waarden uit de request over
        existingShipmentItem.shipment_id = shipmentItem.shipment_id;
        existingShipmentItem.item_id = shipmentItem.item_id;
        existingShipmentItem.amount = shipmentItem.amount;

        await _context.SaveChangesAsync();

        return Ok(existingShipmentItem);
    }

    //verwijdert de shipment item met dit ID 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShipmentItems(int id)
    {
        var existingShipmentItem = await _context.ShipmentItems.FindAsync(id);

        if (existingShipmentItem == null)
        {
            return NotFound();
        }

        _context.ShipmentItems.Remove(existingShipmentItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}

//CRUD : Create, Read, Update, Delete