using CaloriesTracker.DB;
using CaloriesTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using CaloriesTracker.Services;

namespace CaloriesTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly FoodsDbContext _context;
        private readonly IFoodApiService _foodApiService;

        public FoodsController(FoodsDbContext context, IFoodApiService foodApiService)
        {
            _context = context;
            _foodApiService = foodApiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            return Ok(await _context.Foods.AsNoTracking().ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var food = await _context.Foods.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(food);
        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> GetItemByName(string name)
        {
            var food = await _context.Foods.AsNoTracking().FirstOrDefaultAsync(i => i.name == name);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(food);
        }

        [HttpGet("external/search/{name}")]
        public async Task<IActionResult> SearchExternalFood(string name)
        {
            var result = await _foodApiService.SearchFoodAsync(name);

            if (result == null || result.Count == 0)
            {
                return NotFound("Not found in external database");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewItem([FromBody] Foods newItem)
        {
            await _context.Foods.AddAsync(newItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Foods updatedItem)
        {
            var food = await _context.Foods.FirstOrDefaultAsync(i => i.Id == id);
            if (food == null)
            {
                return NotFound();
            }
            food.name = updatedItem.name;
            food.calories = updatedItem.calories;
            food.protein = updatedItem.protein;
            food.fats = updatedItem.fats;
            food.carbs = updatedItem.carbs;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchItem(int id,[FromBody]JsonPatchDocument<Foods> patchedItem)
        {
            var food = await _context.Foods.FirstOrDefaultAsync(i => i.Id == id);
            if(food == null)
            {
                return NotFound();
            }

            patchedItem.ApplyTo(food, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var food = await _context.Foods.FirstOrDefaultAsync(i => i.Id == id);
            if (food == null)
            {
                return NotFound();
            }
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
