using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CaloriesTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
            return Ok(new {Message = "Profile updated successfully!"});
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

                var userId = int.Parse(userIdClaim);
                var exists = await _context.userProfiles.AnyAsync(p => p.UserId == userId);
                return Ok(new { hasProfile = exists });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CheckProfile Error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var profile = await _context.userProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return NotFound("Profile not found");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(new
            {
                email = user?.email,
                weight = profile.Weight,
                height = profile.Height,
                age = profile.Age,
                goal = profile.Goal.ToString() 
            });
        }
        
    }
}
