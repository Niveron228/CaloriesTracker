using CaloriesTracker.DB;
using CaloriesTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using System.Text.RegularExpressions;
using System.Globalization;
using CaloriesTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using CaloriesTracker.Services.FoodsServices;

namespace CaloriesTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFoodApiService _foodApiService;
        private readonly IFoodService _foodService;
        private IMealLogService _mealLogService;

        public FoodsController(AppDbContext context, IFoodApiService foodApiService,IFoodService foodService, IMealLogService mealLogService)
        {
            _context = context;
            _foodApiService = foodApiService;
            _foodService = foodService;
            _mealLogService = mealLogService;
        }

        // GET

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _foodService.GetAllItemsAsync();
            return Ok(items);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _foodService.GetItemByIdAsync(id);
            if (item == null) return NotFound("Food not found");
            return Ok(item);
        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> GetItemByName(string name)
        {
            var items = await _foodService.GetItemByNameAsync(name);
            if (items == null || items.Count == 0) return NotFound("Food not found");
            return Ok(items);
        }

        [HttpGet("daily-stats/{date}")]
        public async Task<IActionResult> GetDailyStats(DateTime date)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);

            var dailySummary = await _mealLogService.GetDailyStatsAsync(currentUserId,date);
            return Ok(dailySummary);
        }

        [Authorize(Roles = "Admin")]
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

        // POST

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddNewItem([FromBody] Foods newItem)
        {
            var createdItem = await _foodService.AddNewItemAsync(newItem);
            return CreatedAtAction(nameof(GetItemById), new { id = createdItem.Id }, createdItem);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        public async Task<IActionResult> ImportExternalFood([FromBody] ExternalFoodDto dto)
        {
            var newLocalFood = await _foodApiService.ImportExternalFoodAsync(dto);

            if (newLocalFood == null) {
                return Conflict(new { message = "This product is already exist in db" });
            }

            return Ok(newLocalFood);
        }

        [HttpPost("add-meal")]
        public async Task<IActionResult> AddMealToLog([FromBody] AddMealDto request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Unauthorized!");
            int currentUserId = int.Parse(userIdClaim.Value);

            var result = await _mealLogService.AddMealToLogAsync(currentUserId, request);

            if (result == null)
            {
                return NotFound(new { Message = "Food not found in local or external database." });
            }

            return Ok(new
            {
                Message = "Meal added!",
                Date = request.LogDate.ToString("yyyy-MM-dd"),
                Meal = request.MealType.ToString(),
                FoodLogged = result.FoodName,
                Amount = $"{request.Grams}g",
                CalculatedMacros = new
                {
                    Calories = Math.Round(result.Calories, 1),
                    Protein = Math.Round(result.Protein, 1),
                    Fat = Math.Round(result.Fat, 1),
                    Carbs = Math.Round(result.Carbs, 1)
                }
            });
        }

        // PUT & PATCH

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Foods updatedItem)
        {
            var item = await _foodService.UpdateItemAsync(id, updatedItem);
            if (item == null) return NotFound("Food not found");
            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchItem(int id, [FromBody] JsonPatchDocument<Foods> patchedItem)
        {
            var item = await _foodService.PatchItemAsync(id, patchedItem);
            if (item == null) return NotFound("Food not found");
            return Ok(item);
        }


        // DELETE

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var isDeleted = await _foodService.DeleteItemAsync(id);
            if (!isDeleted) return NotFound("Food not found");
            return Ok(new { message = "Food successfully deleted" });
        }

        [HttpDelete("delete-meal/{logId}")]
        public async Task<IActionResult> DeleteMealLog(int logId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim.Value);

            bool isDeleted = await _mealLogService.DeleteMealLogAsync(currentUserId, logId);

            if (!isDeleted)
            {
                return NotFound(new { Message = "Not found or you don't have permission!" });
            }

            return Ok(new { Message = "Product removed!" });
        }
    }
}
