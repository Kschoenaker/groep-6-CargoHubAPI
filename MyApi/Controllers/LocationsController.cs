//Opslaglocaties beheren

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

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


    [HttpGet("id/{id:int}")]
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


    [HttpGet("warehouse_id/{warehouseId:int}")]
    public async Task<IActionResult> GetLocationsByWarehouseId(int warehouseId)
    {
        var locations = await _context.Locations
            .Where(location => location.warehouse_id == warehouseId)
            .ToListAsync();

        return Ok(locations);
    }
    

}