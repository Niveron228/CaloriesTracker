using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace CaloriesTracker.Services.FoodsServices
{
    public interface IFoodApiService
    {
        Task<List<ExternalFoodDto>> SearchFoodAsync(string query);
        Task<Foods?> ImportExternalFoodAsync(ExternalFoodDto dto);
        Task SaveExternalListToDbAsync(List<ExternalFoodDto> externalList);
        double ParseNutritionValue(string description, string key);
    }
}
