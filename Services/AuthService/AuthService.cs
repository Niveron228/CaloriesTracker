using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CaloriesTracker.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResultDto> RegisterAsync(UserRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.email == request.email))
            {
                return new AuthResultDto { IsSuccess = false, Message = "User with this email already exist." };
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.password);
            string assignedRole = request.email.ToLower() == "admin123@test.com" ? "Admin" : "User";

            var user = new Users
            {
                email = request.email,
                passwordHash = passwordHash,
                Role = assignedRole
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                IsSuccess = true,
                Message = $"Success Registration!, Your role is: {user.Role}"
            };
        }

        private string CreateToken(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResultDto> LoginAsync(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == request.email);
            if (user == null)
            {
                return new AuthResultDto { IsSuccess = false, Message = "Incorrect email" };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.password, user.passwordHash))
            {
                return new AuthResultDto { IsSuccess = false, Message = "Incorrect Email or password!" };
            }

            string token = CreateToken(user);

            return new AuthResultDto
            {
                IsSuccess = true,
                Token = token, 
                Message = $"Welcome back {user.email}!, your token saved automatically in cookies!"
            };
        }
    }
}
