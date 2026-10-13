//Klanten beheren

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v2/clients")]
public class ClientsController : ControllerBase
{
    public readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> getClients()
    {
        var clients = await _context.Clients.ToListAsync();

        return Ok(clients);
    }
}