using CaloriesTracker.DTOs;

namespace CaloriesTracker.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(UserRegisterDto request);
        Task<AuthResultDto> LoginAsync(UserLoginDto request);
    }
}
