using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.Services.UserProfileServices
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _context;

        public UserProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateProfileAsync(int userId, UserProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var profile = await _context.userProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                profile = new UserProfile { UserId = userId };
                _context.userProfiles.Add(profile);
            }

            profile.Weight = dto.Weight;
            profile.Height = dto.Height;
            profile.Age = dto.Age;
            profile.Goal = (GoalType)dto.Goal;

            double bmr = (10 * dto.Weight) + (6.25 * dto.Height) - (5 * dto.Age) + 5;

            if (profile.Goal == GoalType.LoseWeight) bmr -= 500;
            else if (profile.Goal == GoalType.GainWeight) bmr += 500;

            if (user != null)
            {
                user.DailyCaloriesGoal = (int)bmr;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckProfileExistsAsync(int userId)
        {
            return await _context.userProfiles.AnyAsync(p => p.UserId == userId);
        }

        public async Task<UserProfileResponseDto?> GetProfileAsync(int userId)
        {
            var profile = await _context.userProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return new UserProfileResponseDto
            {
                Email = user?.email ?? string.Empty,
                Weight = profile.Weight,
                Height = profile.Height,
                Age = profile.Age,
                Goal = profile.Goal.ToString()
            };
        }
    }
}
