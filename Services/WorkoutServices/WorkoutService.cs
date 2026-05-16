using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.Services.WorkoutServices
{
    public class WorkoutService : IWorkoutService
    {
        private readonly AppDbContext _context;

        public WorkoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExerciseResponseDto>> GetExercisesAsync(int userId)
        {
            return await _context.exercises
                .Where(e => e.UserId == null || e.UserId == userId)
                .Select(e => new ExerciseResponseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    MuscleGroup = e.MuscleGroup
                })
                .ToListAsync();
        }

        public async Task<List<ExerciseMaxWeightDto>> GetExerciseMaxWeightsAsync(int userId)
        {
            return await _context.workoutsSet
                .Include(s => s.Exercise)
                .Where(s => s.Workout.UserId == userId)
                .GroupBy(s => s.ExerciseId)
                .Select(g => new ExerciseMaxWeightDto
                {
                    ExerciseId = g.Key,
                    ExerciseName = g.Select(s => s.Exercise.Name).FirstOrDefault(),
                    MaxWeight = g.Max(s => s.Weight)
                })
                .ToListAsync();
        }

        public async Task<int> AddSetAsync(int userId, AddWorkoutSetDto request)
        {
            int finalExerciseId;

            if (request.ExerciseId.HasValue && request.ExerciseId > 0)
            {
                finalExerciseId = request.ExerciseId.Value;
            }
            else
            {
                var existingCustomEx = await _context.exercises
                    .FirstOrDefaultAsync(e => e.UserId == userId
                                           && e.Name.ToLower() == request.NewExerciseName.ToLower()
                                           && e.MuscleGroup == request.MuscleGroup);

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

            var workout = await _context.workouts
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Date.Date == request.Date.Date);

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

            return workout.Id;
        }

        public async Task<List<WorkoutHistoryResponse>?> GetWorkoutByDateAsync(int userId, DateTime date)
        {
            var workout = await _context.workouts
                .Include(w => w.WorkoutSets)
                .ThenInclude(s => s.Exercise)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Date.Date == date.Date);

            if (workout == null)
            {
                return null;
            }

            return workout.WorkoutSets.Select(s => new WorkoutHistoryResponse
            {
                SetId = s.Id,
                ExerciseId = s.ExerciseId,
                ExerciseName = s.Exercise.Name,
                MuscleGroup = s.Exercise.MuscleGroup,
                Weight = s.Weight,
                Reps = s.Reps,
                Date = workout.Date
            }).ToList();
        }

        public async Task<bool> DeleteSetAsync(int userId, int setId)
        {
            var workoutSet = await _context.workoutsSet
                .Include(ws => ws.Workout)
                .FirstOrDefaultAsync(ws => ws.Id == setId && ws.Workout.UserId == userId);

            if (workoutSet == null) return false;

            _context.workoutsSet.Remove(workoutSet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
