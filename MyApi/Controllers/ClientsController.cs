//Klanten beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v1/clients")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _context.Clients.ToListAsync();

        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(x => x.id == id);

        if (client == null)
        {
            return NotFound();
        }

        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient(Client client)
    {
        client.created_at = DateTime.UtcNow;
        client.updated_at = DateTime.UtcNow;

        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        return Ok(client);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, Client client)
    {
        var existingClient = await _context.Clients.FirstOrDefaultAsync(x => x.id == id);

        if (existingClient == null)
        {
            return NotFound();
        }

        existingClient.name = client.name;
        existingClient.address = client.address;
        existingClient.city = client.city;
        existingClient.zip_code = client.zip_code;
        existingClient.province = client.province;
        existingClient.country = client.country;
        existingClient.contact_name = client.contact_name;
        existingClient.contact_phone = client.contact_phone;
        existingClient.contact_email = client.contact_email;
        //existingClient.updated_at = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingClient);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(x => x.id == id);

        if (client == null)
        {
            return NotFound();
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return Ok();
    }


}