using CaloriesTracker.DTOs;

namespace CaloriesTracker.Services
{
    public interface IFoodApiService
    {
        Task<List<ExternalFoodDto>> SearchFoodAsync(string query);
    }
}
