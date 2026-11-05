
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
public class ItemController : ControllerBase
{
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // DATABASECONTEXT

    private readonly AppDbContext _context;

    public ItemController(AppDbContext context)
    {
        _context = context;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // 1. GET - ALLE ITEMS OPHALEN
    // GET /api/v2/item

    [HttpGet]
    [Route("api/v2/item")]
    public async Task<IActionResult> GetItems()
    {
        var items = await _context.item.ToListAsync();

        return Ok(items);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // 2. GET - ITEM OPHALEN OP ID
    // GET /api/v2/item/{id}

    [HttpGet]
    [Route("api/v2/item/{id}")]
    public async Task<IActionResult> GetItemById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var item = await _context.item.FindAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // 3. POST - NIEUW ITEM AANMAKEN
    // POST /api/v2/item

    [HttpPost]
    [Route("api/v2/item")]
    public async Task<IActionResult> Create([FromBody] Item newItem)
    {
        if (newItem == null)
        {
            return BadRequest();
        }

        _context.item.Add(newItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetItemById),
            new { id = newItem.Id },
            newItem
        );
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // 4. PUT - BESTAAND ITEM AANPASSEN
    // PUT /api/v2/item/{id}

    [HttpPut]
    [Route("api/v2/item/{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] Item updatedItem)
    {
        if (id <= 0 || updatedItem == null || id != updatedItem.Id)
        {
            return BadRequest();
        }

        var existingItem = await _context.item.FindAsync(id);

        if (existingItem == null)
        {
            return NotFound();
        }

        // Update de eigenschappen van het bestaande item
        existingItem.Code = updatedItem.Code;
        existingItem.Description = updatedItem.Description;
        existingItem.Item_Type_Id = updatedItem.Item_Type_Id;

        _context.item.Update(existingItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // 5. DELETE - ITEM VERWIJDEREN
    // DELETE /api/v2/item/{id}

    [HttpDelete]
    [Route("api/v2/item/{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var item = await _context.item.FindAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        _context.item.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // 6. GERELATEERDE GEGEVENS

    // TODO: GET /api/v2/item/{id}/inventory
    // Voorraadgegevens van een item ophalen.

    // TODO: GET /api/v2/item/{id}/inventory/totals
    // Totale voorraadgegevens van een item ophalen.
}
