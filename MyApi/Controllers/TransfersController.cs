//Verplaatsingen tussen locaties beheren en committen

//Magazijnen beheren
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

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
}