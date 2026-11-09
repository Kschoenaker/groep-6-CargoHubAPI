// Artikelgroepen beheren

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

    public class ItemGroupsController : ControllerBase
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // DATABASECONTEXT

        private readonly AppDbContext _context;

        public ItemGroupsController(AppDbContext context)
        {
            _context = context;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 1. GET - ALLE ARTIKELGROEPEN OPHALEN
        // GET /api/v2/itemgroups

        [HttpGet]
        [Route("api/v2/itemgroups")]
        public async Task<IActionResult> GetItemGroups()
        {
            var itemGroups = await _context.itemGroups.ToListAsync();
            return Ok(itemGroups);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 2. GET - ARTIKELGROEP OPHALEN OP ID
        // GET /api/v2/itemgroups/{id}

        [HttpGet]
        [Route("api/v2/itemgroups/{id}")]
        public async Task<IActionResult> GetItemGroupById(int id)
        {
            var itemGroup = await _context.itemGroups.FindAsync(id);

            if (id <= 0)
            {
                return BadRequest();
            }

            if (itemGroup == null)
            {
                return NotFound();
            }

            return Ok(itemGroup);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 3. POST - NIEUWE ARTIKELGROEP AANMAKEN
        // POST /api/v2/itemgroups

        [HttpPost]
        [Route("api/v2/itemgroups")]
        public async Task<IActionResult> Create([FromBody] ItemGroup newItemGroup)
        {
            return Ok();
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 4. PUT - BESTAANDE ARTIKELGROEP AANPASSEN
        // PUT /api/v2/itemgroups/{id}
        [HttpPut]
        [Route("api/v2/itemgroups/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemGroup updatedItemGroup)
        {
            if (id <= 0 || updatedItemGroup == null || id != updatedItemGroup.Id)
            {
                return BadRequest();
            }

            var existingItemGroup = await _context.itemGroups.FindAsync(id);

            if (existingItemGroup == null)
            {
                return NotFound();
            }

            existingItemGroup.Name = updatedItemGroup.Name;
            existingItemGroup.Description = updatedItemGroup.Description;

            _context.itemGroups.Update(existingItemGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 5. DELETE - ARTIKELGROEP VERWIJDEREN
        // DELETE /api/v2/itemgroups/{id}
        [HttpDelete]
        [Route("api/v2/itemgroups/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var existingItemGroup = await _context.itemGroups.FindAsync(id);

            if (existingItemGroup == null)
            {
                return NotFound();
            }

            _context.itemGroups.Remove(existingItemGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}