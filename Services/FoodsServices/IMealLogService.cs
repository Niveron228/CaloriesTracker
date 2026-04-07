using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace CaloriesTracker.Services.FoodsServices
{
    public interface IMealLogService
    {
        Task<MealLogResultDto?> AddMealToLogAsync(int userId, AddMealDto request);
        Task<DailyStatsDto> GetDailyStatsAsync(int userId,DateTime date);
        Task<bool> DeleteMealLogAsync(int userId,int logId);
    }
}
