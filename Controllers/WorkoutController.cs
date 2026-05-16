using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using CaloriesTracker.Services;
using CaloriesTracker.Services.WorkoutServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkoutController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWorkoutService _workoutService;

        public WorkoutController(AppDbContext context, IWorkoutService workoutService)
        {
            _context = context;
            _workoutService = workoutService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        }

        // GET

        [HttpGet("exercises")]
        public async Task<IActionResult> GetExercises()
        {
            var exercises = await _workoutService.GetExercisesAsync(GetCurrentUserId());
            return Ok(exercises);
        }

        [HttpGet("exercise-max-weights")]
        public async Task<IActionResult> GetExerciseMaxWeights()
        {
            var maxWeights = await _workoutService.GetExerciseMaxWeightsAsync(GetCurrentUserId());
            return Ok(maxWeights);
        }


        [HttpGet("history/{date}")]
        public async Task<IActionResult> GetWorkoutByDate(DateTime date)
        {
            var result = await _workoutService.GetWorkoutByDateAsync(GetCurrentUserId(), date);

            if (result == null)
            {
                return NotFound(new { Message = "No training on this day" });
            }

            return Ok(result);
        }

        // POST

        [HttpPost("add-set")]
        public async Task<IActionResult> AddSet([FromBody] AddWorkoutSetDto request)
        {
            var workoutId = await _workoutService.AddSetAsync(GetCurrentUserId(), request);
            return Ok(new
            {
                Message = "Set added successfully",
                WorkoutId = workoutId
            });
            
        }

        // DELETE

        [HttpDelete("delete-set/{setId}")]
        public async Task<IActionResult> DeleteSet(int setId)
        {
            var isDeleted = await _workoutService.DeleteSetAsync(GetCurrentUserId(), setId);

            if (!isDeleted)
            {
                return NotFound(new { Message = "Set not found or you don't have access to remove it!" });
            }

            return Ok(new { Message = "Set removed!" });
        }

    }
}
