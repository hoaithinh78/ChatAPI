using ChatR.Data;
using ChatR.DTOs.Friends;
using ChatR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace ChatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendRequestController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FriendRequestController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet("friend-requests")]
        public async Task<IActionResult> GetFriendRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var friendRequests = await _dbContext.FriendRequests
                .Where(r => r.ReceiverId == userId && r.Status == 0)
                .Select(r => new
                {
                    r.RequestId,
                    SenderId = r.SenderId,
                    SenderUsername = _dbContext.Users.Where(u => u.UserId == r.SenderId).Select(u => u.Username).FirstOrDefault(),
                    r.CreatedAt
                })
                .ToListAsync();
            return Ok(friendRequests);
        }
        [HttpGet("sent-requests")]
        public async Task<IActionResult> GetSentRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var sentRequests = await _dbContext.FriendRequests
                .Where(r => r.SenderId == userId && r.Status == 0)
                .Select(r => new
                {
                    r.RequestId,
                    ReceiverId = r.ReceiverId,
                    ReceiverUsername = _dbContext.Users
                        .Where(u => u.UserId == r.ReceiverId)
                        .Select(u => u.Username)
                        .FirstOrDefault(),
                    ReceiverDisplayName = _dbContext.Users
                        .Where(u => u.UserId == r.ReceiverId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault(),
                    ReceiverAvatarUrl = _dbContext.Users
                        .Where(u => u.UserId == r.ReceiverId)
                        .Select(u => u.AvatarUrl)
                        .FirstOrDefault(),
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(sentRequests);
        }
        [HttpPost("send-friend-request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto friendRequestDto)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var receiverId = friendRequestDto.ReceiverId;

            if (senderId == receiverId)
            {
                return BadRequest("Không thể gửi yêu cầu kết bạn cho chính mình.");
            }

            var existingRequest = await _dbContext.FriendRequests
                .FirstOrDefaultAsync(r => r.SenderId == senderId && r.ReceiverId == receiverId);

            if (existingRequest != null)
            {
                return BadRequest("Bạn đã gửi yêu cầu kết bạn rồi.");
            }

            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.FriendRequests.Add(friendRequest);
            await _dbContext.SaveChangesAsync();

            return Ok("Yêu cầu kết bạn đã được gửi.");
        }

        [HttpPost("accept-friend-request")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] int requestId)
        {
            // Lấy yêu cầu kết bạn chưa được chấp nhận
            var request = await _dbContext.FriendRequests
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.Status == 0);

            if (request == null)
            {
                return NotFound("Yêu cầu kết bạn không tồn tại hoặc đã được chấp nhận.");
            }

            // Kiểm tra nếu đã là bạn
            var existingFriend = await _dbContext.Friends
                .FirstOrDefaultAsync(f => (f.UserId == request.SenderId && f.FriendId == request.ReceiverId) ||
                                          (f.UserId == request.ReceiverId && f.FriendId == request.SenderId));

            if (existingFriend != null)
            {
                return BadRequest("Bạn đã là bạn của nhau.");
            }

            // Chỉ thêm 1 bản ghi, đảm bảo user_id < friend_id
            int userId = Math.Min(request.SenderId, request.ReceiverId);
            int friendId = Math.Max(request.SenderId, request.ReceiverId);

            var friend = new Friend
            {
                UserId = userId,
                FriendId = friendId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Friends.Add(friend);

            // Cập nhật trạng thái yêu cầu kết bạn
            request.Status = 1;
            _dbContext.FriendRequests.Update(request);

            await _dbContext.SaveChangesAsync();

            return Ok("Yêu cầu kết bạn đã được chấp nhận.");
        }
        [HttpDelete("cancel-friend-request/{requestId}")]
        public async Task<IActionResult> CancelFriendRequest(int requestId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var request = await _dbContext.FriendRequests
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.SenderId == userId && r.Status == 0);

            if (request == null)
                return NotFound("Không tìm thấy lời mời để hủy.");

            _dbContext.FriendRequests.Remove(request);
            await _dbContext.SaveChangesAsync();

            return Ok("Đã hủy lời mời kết bạn.");
        }
        [HttpPost("reject-friend-request")]
        public async Task<IActionResult> RejectFriendRequest([FromBody] int requestId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var request = await _dbContext.FriendRequests
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.ReceiverId == userId && r.Status == 0);

            if (request == null)
                return NotFound("Yêu cầu kết bạn không tồn tại.");

            _dbContext.FriendRequests.Remove(request);
            await _dbContext.SaveChangesAsync();

            return Ok("Đã từ chối yêu cầu kết bạn.");
        }
    }
}
