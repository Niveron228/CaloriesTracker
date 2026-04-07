using CaloriesTracker.DTOs;

namespace CaloriesTracker.Services.UserProfileServices
{
    public interface IUserProfileService
    {
        Task<bool> UpdateProfileAsync(int userId, UserProfileDto dto);
        Task<bool> CheckProfileExistsAsync(int userId);
        Task<UserProfileResponseDto?> GetProfileAsync(int userId);
    }
}
