//Artikeltypen beheren

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
// 1. GET - ALLE ARTIKELTYPEN OPHALEN
// 2. GET - ARTIKELTYPE OPHALEN OP ID
// 3. GET - ALLE ITEMS VAN EEN ARTIKELTYPE OPHALEN -  api/v1/item_types/{id}/items
// 4. POST - NIEUW ARTIKELTYPE AANMAKEN
// 5. PUT - BESTAAND ARTIKELTYPE AANPASSEN
// 6. DELETE - ARTIKELTYPE VERWIJDEREN
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------



namespace MyApi.Controllers
{
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//CLASS ITEMTYPESCONTROLLER
    public class ItemTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//CONSTRUCTOR 
        public ItemTypesController(AppDbContext context)
        {
            _context = context;
        }

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//GET ALL ITEMTYPES
        [HttpGet]
        [Route("api/v2/itemtypes")]
        public async Task<IActionResult> GetItemTypes()
        {
            var itemTypes = await _context.itemTypes.ToListAsync();
            return Ok(itemTypes);
        }

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//GET ALL ITEMTYPES BY ID
        [HttpGet] 
        [Route("api/v2/itemtypes/{id}")]
        public async Task<IActionResult> GetItemTypeById(int id)
        {
            var itemTypes = await _context.itemTypes.FindAsync(id);

            if (id <= 0)
            {
                return BadRequest();
            }

            if (itemTypes == null)
            {
                return NotFound();
            }

            return Ok(itemTypes);
        }

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//GET ALL ITEMS BY ITEMTYPE ID
        [HttpGet]
        [Route("/api/v2/itemtypes/{id}/items")]
        public async Task<IActionResult> GetItemsByItemTypeId(int id)
        {

            var itemType = await _context.itemTypes.FindAsync(id);

            if (itemType == null)
            {
                return NotFound();
            }

            // Haal de items op die bij dit itemtype horen
            var items = await _context.item
                .Where(i => i.Item_Type_Id == id)
                .ToListAsync();

            return Ok(items);
        }

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//GET ALL ITEMS BY ITEMTYPE ID

[HttpGet]
[Route("/api/v2/itemtypes/{id}/itemlines")]
public async Task<IActionResult> GetItemLinesByItemTypeId(int id)
{
    // Controleer of het itemtype bestaat
    var itemType = await _context.itemTypes.FindAsync(id);

    if (itemType == null)
    {
        return NotFound();
    }

    // Zoek de items die bij dit itemtype horen
    var itemLines = await _context.item
        .Where(i => i.Item_Type_Id == id)
        .Join(
            _context.itemLines,
            item => item.Item_Line_Id,
            itemLine => itemLine.Id,
            (item, itemLine) => itemLine
        )
        .Distinct()
        .ToListAsync();

    return Ok(itemLines);
}


//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//POST CREATE NEW ITEM POST
        [HttpPost]
        [Route("api/v2/itemtypes")] 
        public async Task<IActionResult> Create([FromBody] ItemType newItemType) 
        {
            return Ok();
        }

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//PUT UPDATE ITEM
        [HttpPut]
        [Route("api/v2/itemtypes/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemType updatedItemType)
        {
            return Ok();
        }
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------
//PUT UPDATE ITEM
        [HttpDelete]
        [Route("api/v2/itemtypes/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok();
        }
    }
}
