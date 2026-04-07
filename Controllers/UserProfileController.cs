using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using CaloriesTracker.Services.UserProfileServices;
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
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(AppDbContext context,IUserProfileService userProfileService)
        {
            _context = context;
            _userProfileService = userProfileService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        // GET

        [HttpGet("check")]
        public async Task<IActionResult> CheckProfile()
        {
            try
            {
                var exists = await _userProfileService.CheckProfileExistsAsync(GetCurrentUserId());
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
            var profile = await _userProfileService.GetProfileAsync(GetCurrentUserId());

            if (profile == null) return NotFound("Profile not found");

            return Ok(profile);
        }


        // POST

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
            await _userProfileService.UpdateProfileAsync(GetCurrentUserId(), dto);
            return Ok(new { Message = "Profile updated successfully!" });
        }

    }
}
