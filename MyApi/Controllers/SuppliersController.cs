//Leveranciers beheren

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v1/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _context;

    public SuppliersController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        var suppliers = await _context.Suppliers.ToListAsync();

        return Ok(suppliers);
    }

    [HttpGet("{id}")] //get id
    public async Task<IActionResult> GetSupplier(int id)
    {
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.id == id);

        if (supplier == null)
        {
            return NotFound();
        }
        return Ok(supplier);
    }

    [HttpPost] //maak supplier
    public async Task<IActionResult> CreateSupplier(Supplier supplier)
    {
        supplier.created_at = DateTime.UtcNow; 
        supplier.updated_at = DateTime.UtcNow;

        await _context.Suppliers.AddAsync(supplier);
        await _context.SaveChangesAsync();

        return Ok(supplier);
    }

    [HttpPut("{id}")] //verander dit supplier
    public async Task<IActionResult> UpdateSupplier(int id, Supplier supplier)
    {
        var existingSupplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.id == id);

        if (existingSupplier == null)
        {
            return NotFound();
        }

        existingSupplier.province = supplier.province;
        existingSupplier.country = supplier.country;
        existingSupplier.contact_name = supplier.contact_name;
        existingSupplier.phone_number = supplier.phone_number;
        existingSupplier.reference = supplier.reference;
        existingSupplier.updated_at = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingSupplier);
    }

    //delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.id == id);

        if (supplier == null)
        {
            return NotFound();
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return Ok();
    }


}
