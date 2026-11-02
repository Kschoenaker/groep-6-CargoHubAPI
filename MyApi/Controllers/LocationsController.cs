//Opslaglocaties beheren

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v2/locations")]
public class LocationsController : ControllerBase
{

    private readonly AppDbContext _context;

    public LocationsController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> getLocations()
    {
        var locations = await _context.Locations.ToListAsync();

        return Ok(locations);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLocationById(int id)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(location => location.id == id);

        if (location is null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] Location location)
    {
        var warehouseExists = await _context.Warehouses
            .AnyAsync(warehouse => warehouse.id == location.warehouse_id);

        if (!warehouseExists)
        {
            return BadRequest("Warehouse does not exist");
        }

        location.id = 0;
        location.created_at = DateTime.UtcNow;
        location.updated_at = DateTime.UtcNow;

        _context.Locations.Add(location);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Location code already exists in this warehouse");
        }

        return CreatedAtAction(nameof(GetLocationById), new { id = location.id }, location);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location updated)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(location => location.id == id);

        if (location is null)
        {
            return NotFound();
        }

        var warehouseExists = await _context.Warehouses
            .AnyAsync(warehouse => warehouse.id == updated.warehouse_id);

        if (!warehouseExists)
        {
            return BadRequest("Warehouse does not exist");
        }

        location.warehouse_id = updated.warehouse_id;
        location.code = updated.code;
        location.name = updated.name;
        location.updated_at = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Location code already exists in this warehouse");
        }

        return Ok(location);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(location => location.id == id);

        if (location is null)
        {
            return NotFound();
        }

        _context.Locations.Remove(location);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Location is still used by transfers or inventories");
        }

        return NoContent();
    }
}