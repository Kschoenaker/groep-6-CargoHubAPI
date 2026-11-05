// Artikelregels beheren

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    public class ItemLinesController : ControllerBase
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // DATABASECONTEXT
        private readonly AppDbContext _context;

        public ItemLinesController(AppDbContext context)
        {
            _context = context;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 1. GET - ALLE ARTIKELREGELS OPHALEN
        // GET /api/v2/itemlines
        [HttpGet]
        [Route("api/v2/itemlines")]
        public async Task<IActionResult> GetItemLines()
        {
            var itemLines = await _context.itemLines.ToListAsync();
            return Ok(itemLines);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 2. GET - ARTIKELREGEL OPHALEN OP ID
        // GET /api/v2/itemlines/{id}
        [HttpGet]
        [Route("api/v2/itemlines/{id}")]
        public async Task<IActionResult> GetItemLineById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var itemLine = await _context.itemLines.FindAsync(id);

            if (itemLine == null)
            {
                return NotFound();
            }

            return Ok(itemLine);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 3. POST - NIEUWE ARTIKELREGEL AANMAKEN
        // POST /api/v2/itemlines
        [HttpPost]
        [Route("api/v2/itemlines")]
        public async Task<IActionResult> Create([FromBody] ItemLine newItemLine)
        {
            if (newItemLine == null)
            {
                return BadRequest();
            }

            _context.itemLines.Add(newItemLine);
            await _context.SaveChangesAsync();

            return Ok(newItemLine);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 4. PUT - BESTAANDE ARTIKELREGEL AANPASSEN
        // PUT /api/v2/itemlines/{id}
        [HttpPut]
        [Route("api/v2/itemlines/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemLine updatedItemLine)
        {
            if (id <= 0 || updatedItemLine == null || id != updatedItemLine.Id)
            {
                return BadRequest();
            }

            var existingItemLine = await _context.itemLines.FindAsync(id);
            if (existingItemLine == null)
            {
                return NotFound();
            }

            existingItemLine.Name = updatedItemLine.Name;
            existingItemLine.Description = updatedItemLine.Description;
                    existingItemLine.UpdatedAt = DateTime.UtcNow;

            _context.itemLines.Update(existingItemLine);
            await _context.SaveChangesAsync();

            return Ok(existingItemLine);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 5. DELETE - ARTIKELREGEL VERWIJDEREN
        // DELETE /api/v2/itemlines/{id}
        [HttpDelete]
        [Route("api/v2/itemlines/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingItemLine = await _context.itemLines.FindAsync(id);
            if (existingItemLine == null)
            {
                return NotFound();
            }

            _context.itemLines.Remove(existingItemLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}