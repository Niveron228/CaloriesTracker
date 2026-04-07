using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.Services.FoodsServices
{
    public class MealLogService : IMealLogService
    {
        private readonly AppDbContext _context;
        private readonly IFoodApiService _foodApiService;

        public MealLogService(AppDbContext context, IFoodApiService foodApiService)
        {
            _context = context;
            _foodApiService = foodApiService;
        }

        public async Task<bool> DeleteMealLogAsync(int userId, int logId)
        {
            var mealLog = await _context.mealLogs
                .FirstOrDefaultAsync(m => m.Id == logId && m.UserId == userId);

            if (mealLog == null)
            {
                return false; 
            }

            _context.mealLogs.Remove(mealLog);
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task<MealLogResultDto?> AddMealToLogAsync(int userId, AddMealDto request)
        {
            var searchTerm = request.FoodName.ToLower();

            var food = await _context.Foods.FirstOrDefaultAsync(f => f.name.ToLower() == searchTerm);

            if (food == null)
            {
                var externalList = await _foodApiService.SearchFoodAsync(request.FoodName);

                if (externalList == null || externalList.Count == 0)
                {
                    return null; 
                }
                await _foodApiService.SaveExternalListToDbAsync(externalList);

                food = await _context.Foods.FirstOrDefaultAsync(f => f.name.ToLower() == searchTerm)
                    ?? await _context.Foods.FirstOrDefaultAsync(f => f.name.ToLower().Contains(searchTerm));

                if (food == null) return null;
            }

            var newMealLog = new MealLog
            {
                UserId = userId,
                FoodId = food.Id,
                Grams = request.Grams,
                Type = request.MealType,
                LogDate = request.LogDate
            };

            _context.mealLogs.Add(newMealLog);
            await _context.SaveChangesAsync();

            return new MealLogResultDto
            {
                FoodName = food.name,
                Calories = (food.calories * request.Grams) / 100.0,
                Protein = (food.protein * request.Grams) / 100.0,
                Fat = (food.fats * request.Grams) / 100.0,
                Carbs = (food.carbs * request.Grams) / 100.0
            };
        }

        public async Task<DailyStatsDto> GetDailyStatsAsync(int userId, DateTime date)
        {
            var logs = await _context.mealLogs
                .Include(m => m.Food)
                .Where(m => m.UserId == userId
                         && m.LogDate >= date.Date
                         && m.LogDate < date.Date.AddDays(1))
                .ToListAsync();

            var report = Enum.GetValues(typeof(MealLog.MealType))
                .Cast<MealLog.MealType>()
                .Select(type =>
                {
                    var mealItems = logs.Where(l => l.Type == type).ToList();

                    return new MealReportDto
                    {
                        MealName = type.ToString(),
                        TotalCalories = Math.Round(mealItems.Sum(i => (i.Food.calories * i.Grams) / 100.0), 1),
                        TotalProtein = Math.Round(mealItems.Sum(i => (i.Food.protein * i.Grams) / 100.0), 1),
                        TotalFat = Math.Round(mealItems.Sum(i => (i.Food.fats * i.Grams) / 100.0), 1),
                        TotalCarbs = Math.Round(mealItems.Sum(i => (i.Food.carbs * i.Grams) / 100.0), 1),
                        ItemsCount = mealItems.Count,

                        Items = mealItems.Select(item => new MealItemDto
                        {
                            Id = item.Id,
                            Name = item.Food.name,
                            Grams = Convert.ToInt32(item.Grams),
                            Calories = Math.Round((item.Food.calories * item.Grams) / 100.0, 1),
                            Protein = Math.Round((item.Food.protein * item.Grams) / 100.0, 1),
                            Fat = Math.Round((item.Food.fats * item.Grams) / 100.0, 1),
                            Carbs = Math.Round((item.Food.carbs * item.Grams) / 100.0, 1)
                        }).ToList()
                    };
                }).ToList();

            return new DailyStatsDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Meals = report,
                DayTotal = new MacrosDto
                {
                    Calories = Math.Round(logs.Sum(i => (i.Food.calories * i.Grams) / 100.0), 1),
                    Protein = Math.Round(logs.Sum(i => (i.Food.protein * i.Grams) / 100.0), 1),
                    Fat = Math.Round(logs.Sum(i => (i.Food.fats * i.Grams) / 100.0), 1),
                    Carbs = Math.Round(logs.Sum(i => (i.Food.carbs * i.Grams) / 100.0), 1)
                }
            };
        }
    }
}
