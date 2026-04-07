using CaloriesTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.Services.FoodsServices
{
    public interface IFoodService
    {
        Task<Foods?> GetItemByIdAsync(int id);
        Task<List<Foods>> GetAllItemsAsync();
        Task<List<Foods>> GetItemByNameAsync(string name);
        Task<Foods?> UpdateItemAsync(int id, [FromBody] Foods updatedItem);
        Task<Foods?> PatchItemAsync(int id, [FromBody] JsonPatchDocument<Foods> patchedItem);
        Task<bool> DeleteItemAsync(int id);
        Task<Foods> AddNewItemAsync([FromBody] Foods newItem);
    }
}
