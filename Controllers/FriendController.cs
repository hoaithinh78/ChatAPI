using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChatR.Server.Data;
using ChatR.Server.Models;


namespace ChatR.Server.Controllers
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // Lấy UserId từ token

            // Truy vấn bảng Friends để lấy danh sách bạn bè của người dùng
            var friends = await _dbContext.Friends
                .Where(f => f.UserId == userId || f.FriendId == userId)
                .Include(f => f.UserInfo)  // Bao gồm thông tin người dùng (người gửi)
                .Include(f => f.FriendInfo)  // Bao gồm thông tin bạn bè (người nhận)
                .ToListAsync();

            // Nếu không có bạn bè, trả về lỗi NotFound
            if (friends == null || !friends.Any())
            {
                return NotFound("Bạn chưa có bạn bè.");
            }

            // Trả về danh sách bạn bè với thông tin người dùng và bạn bè
            return Ok(friends.Select(f => new
            {
                FriendId = f.UserId == userId ? f.FriendId : f.UserId,
                DisplayName = f.UserId == userId ? f.FriendInfo.DisplayName : f.UserInfo.DisplayName,
                AvatarUrl = f.UserId == userId ? f.FriendInfo.AvatarUrl : f.UserInfo.AvatarUrl
            }));
        }
    }
}