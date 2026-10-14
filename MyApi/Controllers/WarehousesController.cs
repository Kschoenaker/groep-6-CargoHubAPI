//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v2/warehouses")]
public class WarehousesController : ControllerBase
{

    private readonly AppDbContext _context;

    public WarehousesController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> getWareHouses()
    {
        var warehouses = await _context.Warehouses.ToListAsync();

        return Ok(warehouses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWarehouseById(int id)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.id == id);

        if (warehouse is null)
        {
            return NotFound();
        }

        return Ok(warehouse);
    }
}