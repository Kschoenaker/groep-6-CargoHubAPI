//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
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
        var warehouses = await _context.warehouses.ToListAsync();

        return Ok(warehouses);
    }

}

//CRUD : Create, Read, Update, Delete