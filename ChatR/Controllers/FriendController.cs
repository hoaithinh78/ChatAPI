using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChatR.Data;
using ChatR.Models;


namespace ChatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FriendController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet("get-friends")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); 
            var friends = await _dbContext.Friends
                .Where(f => f.UserId == userId || f.FriendId == userId)
                .Include(f => f.UserInfo)  
                .Include(f => f.FriendInfo)  
                .ToListAsync();

            if (friends == null || !friends.Any())
            {
                return NotFound("Bạn chưa có bạn bè.");
            }
            return Ok(friends.Select(f => new
            {
                FriendId = f.UserId == userId ? f.FriendId : f.UserId,
                DisplayName = f.UserId == userId ? f.FriendInfo.DisplayName : f.UserInfo.DisplayName,
                AvatarUrl = f.UserId == userId ? f.FriendInfo.AvatarUrl : f.UserInfo.AvatarUrl
            }));
        }
    }
}