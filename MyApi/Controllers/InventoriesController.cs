//Voorraad per artikel en locatie beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models; // models erbij gehaald

namespace MyApi.Controllers;

[ApiController]
[Route("api/v1/inventories")]
public class InventoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public InventoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet] //haalt items in de inventory
    public async Task<IActionResult> GetInventories() 
    {
        var inventories = await _context.Inventories.ToListAsync();

        return Ok(inventories);
    }

    [HttpGet("{itemId}/{locationId}")] //haalt 1 item met id uit de inventory
    public async Task<IActionResult> GetInventory(int itemId, int locationId)
    {
        var inventory = await _context.Inventories .FirstOrDefaultAsync(x =>
            x.item_id == itemId && x.location_id == locationId);

    if (inventory == null)
    {
        return NotFound();
    }

    return Ok(inventory);
}

    [HttpPost] //maakt een nieuwe inventory record
    public async Task<IActionResult> CreateInventory(Inventory inventory)
    {
        var existingInventory = await _context.Inventories //checkt inventories voor een inventory met dezelfde id
            .FirstOrDefaultAsync(x => x.item_id == inventory.item_id && x.location_id == inventory.location_id);

        if (existingInventory != null)
        {
            return Conflict("Dit inventory bestaat al."); //als inventory al bestaat geef error
        }

        await _context.Inventories.AddAsync(inventory); //maakt inventory aan
        await _context.SaveChangesAsync(); //sla op

        return Ok(inventory);
    }

    [HttpPut("{itemId}/{locationId}")] //pas inventory aan
    public async Task<IActionResult> UpdateInventory(int itemId, int locationId, Inventory inventory)
    {
        var existingInventory = await _context.Inventories
            .FirstOrDefaultAsync(x => x.item_id == itemId && x.location_id == locationId);

        if (existingInventory == null)
        {
            return NotFound(); //kan ik hier iets beter doen? 
        }

        //aanpassen
        existingInventory.quantity_on_hand = inventory.quantity_on_hand;
        existingInventory.quantity_expected = inventory.quantity_expected;
        existingInventory.quantity_ordered = inventory.quantity_ordered;
        existingInventory.quantity_allocated = inventory.quantity_allocated;
        existingInventory.updated_at = DateTime.UtcNow;

        await _context.SaveChangesAsync(); //opslaan

        return Ok(existingInventory);
    }

    [HttpDelete("{itemId}/{locationId}")] //inventory verwijderen
    public async Task<IActionResult> DeleteInventory(int itemId, int locationId)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(x => x.item_id == itemId && x.location_id == locationId);

        if (inventory == null)
        {
            return NotFound();
        }

        _context.Inventories.Remove(inventory); // remove verwijdert 
        await _context.SaveChangesAsync();

        return Ok();
    }


}