using CaloriesTracker.DB;
using Microsoft.AspNetCore.Mvc;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CaloriesTracker.Services.AuthService;

namespace CaloriesTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(AppDbContext context, IConfiguration config, IAuthService authService)
        {
            _context = context;
            _config = config;
            _authService = authService;
        }

        // POST

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddDays(1)
            };

            Response.Cookies.Append("jwt", result.Token, cookieOption);

            return Ok(new { message = result.Message });
        }
    }
}
