//Verplaatsingen tussen locaties beheren en committen

//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/v2/transfers")]
public class TransfersController : ControllerBase
{

    private readonly AppDbContext _context;

    public TransfersController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> getTransfers()
    {
        var transfers = await _context.Transfers.ToListAsync();

        return Ok(transfers);
    }

    [HttpGet("{transferId:int}")]
    public async Task<IActionResult> GetTransferById(int transferId)
    {
        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(transfer => transfer.id == transferId);

        if (transfer is null)
        {
            return NotFound();
        }

        return Ok(transfer);
    }

    [HttpGet("{transferId:int}/items")]
    public async Task<IActionResult> GetTransferItems(int transferId)
    {
        var items = await _context.TransferItems
            .Where(item => item.transfer_id == transferId)
            .Select(item => new { item.item_id, item.amount })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer([FromBody] Transfer transfer)
    {
        var error = await ValidateLocations(transfer);

        if (error is not null)
        {
            return BadRequest(error);
        }

        transfer.id = 0;
        transfer.created_at = DateTime.UtcNow;
        transfer.updated_at = DateTime.UtcNow;

        _context.Transfers.Add(transfer);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Transfer reference already exists");
        }

        return CreatedAtAction(nameof(GetTransferById), new { transferId = transfer.id }, transfer);
    }

    [HttpPut("{transferId:int}")]
    public async Task<IActionResult> UpdateTransfer(int transferId, [FromBody] Transfer updated)
    {
        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(transfer => transfer.id == transferId);

        if (transfer is null)
        {
            return NotFound();
        }

        var error = await ValidateLocations(updated);

        if (error is not null)
        {
            return BadRequest(error);
        }

        transfer.reference = updated.reference;
        transfer.from_location_id = updated.from_location_id;
        transfer.to_location_id = updated.to_location_id;
        transfer.transfer_status = updated.transfer_status;
        transfer.updated_at = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Transfer reference already exists");
        }

        return Ok(transfer);
    }

    [HttpDelete("{transferId:int}")]
    public async Task<IActionResult> DeleteTransfer(int transferId)
    {
        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(transfer => transfer.id == transferId);

        if (transfer is null)
        {
            return NotFound();
        }

        // transfer_items verwijzen naar de transfer, dus die eerst verwijderen
        var items = await _context.TransferItems
            .Where(item => item.transfer_id == transferId)
            .ToListAsync();

        _context.TransferItems.RemoveRange(items);
        _context.Transfers.Remove(transfer);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<string?> ValidateLocations(Transfer transfer)
    {
        if (transfer.from_location_id == transfer.to_location_id)
        {
            return "from_location_id and to_location_id must be different";
        }

        var count = await _context.Locations
            .CountAsync(location => location.id == transfer.from_location_id
                || location.id == transfer.to_location_id);

        if (count != 2)
        {
            return "From or to location does not exist";
        }

        return null;
    }
}