using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using CaloriesTracker.Services;
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

        public WorkoutController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("exercises")]
        public async Task<IActionResult> GetExercises()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            var exercises = await _context.exercises
                .Where(e => e.UserId == null || e.UserId == userId)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.MuscleGroup
                })
                .ToListAsync();

            return Ok(exercises);
        }

        [HttpPost("add-set")]
        public async Task<IActionResult> AddSet([FromBody] AddWorkoutSetDto request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            int finalExerciseId;
            if (request.ExerciseId.HasValue && request.ExerciseId > 0)
            {
                finalExerciseId = request.ExerciseId.Value;
            }
            else
            {
                var existingCustomEx = await _context.exercises
                    .FirstOrDefaultAsync(e => e.UserId == userId && e.Name.ToLower() == request.NewExerciseName.ToLower() && e.MuscleGroup == request.MuscleGroup);

                if (existingCustomEx != null)
                {
                    finalExerciseId = existingCustomEx.Id;
                }

                else 
                {
                    var newEx = new Exercise
                    {
                        Name = request.NewExerciseName,
                        MuscleGroup = request.MuscleGroup,
                        UserId = userId
                    };
                    _context.exercises.Add(newEx);
                    await _context.SaveChangesAsync();
                    finalExerciseId = newEx.Id;
                }

            }

            var workout = await _context.workouts.FirstOrDefaultAsync(w => w.UserId == userId && w.Date.Date == request.Date.Date);

            if (workout == null)
            {
                workout = new Workout
                {
                    UserId = userId,
                    Date = request.Date.Date
                };
                _context.workouts.Add(workout);
                await _context.SaveChangesAsync();
            }

            var workoutSet = new WorkoutSet
            {
                WorkoutId = workout.Id,
                ExerciseId = finalExerciseId,
                Weight = request.Weight,
                Reps = request.Reps
            };

            await _context.workoutsSet.AddAsync(workoutSet);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Set added successfully", WorkoutId = workout.Id });
        }

        [HttpGet("history/{date}")]
        public async Task<IActionResult> GetWorkoutByDate(DateTime date)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            var workout = await _context.workouts
                .Include(w => w.WorkoutSets)
                .ThenInclude(s => s.Exercise)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Date.Date == date.Date);

            if (workout == null)
            {
                return NotFound(new { Message = "No training on this day" });
            }

            var result = workout.WorkoutSets.Select(s => new WorkoutHistoryResponse
            {
                SetId = s.Id,
                ExerciseId = s.ExerciseId,
                ExerciseName = s.Exercise.Name,
                MuscleGroup = s.Exercise.MuscleGroup,
                Weight = s.Weight,
                Reps = s.Reps,
                Date = workout.Date
            }).ToList();

            return Ok(result);
        }

        [HttpDelete("delete-set/{setId}")]
        public async Task<IActionResult> DeleteSet(int setId)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            var workoutSet = await _context.workoutsSet
                .Include(ws => ws.Workout)
                .FirstOrDefaultAsync(ws => ws.Id == setId && ws.Workout.UserId == userId);
            if (workoutSet == null)
            {
                return NotFound(new { Message = "Set not found or you don't have access to remove it!" });
            }

            _context.workoutsSet.Remove(workoutSet);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Set removed!" });
        }

    }
}