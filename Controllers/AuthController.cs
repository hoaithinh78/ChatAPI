// RegisterController.cs
using ChatR.Server.Data;
using ChatR.Server.DTOs;
using ChatR.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == userRegisterDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email đã tồn tại.");
            }

            // Tạo người dùng mới
            var user = new User
            {
                Email = userRegisterDto.Email,
                Username = userRegisterDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password),
                CreatedAt = DateTime.UtcNow,
                Status = 1
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Tạo JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
            {
                return Unauthorized("Thông tin đăng nhập không đúng.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); 
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            user.DisplayName = userUpdateDto.DisplayName;
            user.Bio = userUpdateDto.Bio;
            user.AvatarUrl = userUpdateDto.AvatarUrl;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(user);
        }

        public class UserUpdateDto
        {
            public string DisplayName { get; set; }
            public string Bio { get; set; }
            public string AvatarUrl { get; set; }
        }
    }
}