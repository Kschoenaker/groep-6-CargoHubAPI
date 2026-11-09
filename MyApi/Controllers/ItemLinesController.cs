// Artikelregels beheren

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers
{
    // All endpoints for the Item entity
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
// 1. GET - ALLE ARTIKELGROEPEN OPHALEN
// 2. GET - ARTIKELGROEP OPHALEN OP ID
// 3. GET - ALLE ITEMS VAN EEN ARTIKELGROEP OPHALEN -  api/v1/item_groups/{id}/items
// 4. POST - NIEUWE ARTIKELGROEP AANMAKEN
// 5. PUT - BESTAANDE ARTIKELGROEP AANPASS
// 6. DELETE - ARTIKELGROEP VERWIJDEREN

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------


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

        [HttpGet]
        [Route("api/v2/itemlines/{id}/items")]
        public async Task<IActionResult> GetItemsByItemLineId(int id)
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

            var items = await _context.item
                .Where(i => i.Item_Line_Id == id)
                .ToListAsync();

            return Ok(items);
        }
    }
}