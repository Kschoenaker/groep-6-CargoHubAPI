//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

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

    [HttpGet("{warehouseId:int}/locations")]
    public async Task<IActionResult> GetLocationsByWarehouseId(int warehouseId)
    {
        var locations = await _context.Locations
            .Where(location => location.warehouse_id == warehouseId)
            .ToListAsync();

        return Ok(locations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
    {
        warehouse.id = 0;
        warehouse.created_at = DateTime.UtcNow;
        warehouse.updated_at = DateTime.UtcNow;

        _context.Warehouses.Add(warehouse);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Warehouse code already exists");
        }

        return CreatedAtAction(nameof(GetWarehouseById), new { id = warehouse.id }, warehouse);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse updated)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.id == id);

        if (warehouse is null)
        {
            return NotFound();
        }

        warehouse.code = updated.code;
        warehouse.name = updated.name;
        warehouse.address = updated.address;
        warehouse.city = updated.city;
        warehouse.zip_code = updated.zip_code;
        warehouse.province = updated.province;
        warehouse.country = updated.country;
        warehouse.contact_name = updated.contact_name;
        warehouse.contact_phone = updated.contact_phone;
        warehouse.contact_email = updated.contact_email;
        warehouse.updated_at = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Warehouse code already exists");
        }

        return Ok(warehouse);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.id == id);

        if (warehouse is null)
        {
            return NotFound();
        }

        _context.Warehouses.Remove(warehouse);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Warehouse still has locations");
        }

        return NoContent();
    }
}