using CaloriesTracker.DTOs;

namespace CaloriesTracker.Services.WorkoutServices
{
    public interface IWorkoutService
    {
        Task<List<ExerciseResponseDto>> GetExercisesAsync(int userId);
        Task<List<ExerciseMaxWeightDto>> GetExerciseMaxWeightsAsync(int userId);
        Task<int> AddSetAsync(int userId, AddWorkoutSetDto request);
        Task<List<WorkoutHistoryResponse>?> GetWorkoutByDateAsync(int userId, DateTime date);
        Task<bool> DeleteSetAsync(int userId, int setId);
    }
}
