using System.Security.Claims;
using ChatR.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatR.Controllers
{
    [ApiController]
    [Route("api/friend")]
    [Authorize]
    public class FriendSidebarController : ControllerBase
    {
        private readonly IFriendSidebarService _friendSidebarService;

        public FriendSidebarController(IFriendSidebarService friendSidebarService)
        {
            _friendSidebarService = friendSidebarService;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(claim))
                throw new Exception("Không lấy được userId từ token.");
            return int.Parse(claim);
        }

        [HttpGet("my-friends")]
        public async Task<IActionResult> GetMyFriends()
        {
            var userId = GetCurrentUserId();
            var result = await _friendSidebarService.GetMyFriendsAsync(userId);
            return Ok(result);
        }

        [HttpGet("online-friends")]
        public async Task<IActionResult> GetOnlineFriends()
        {
            var userId = GetCurrentUserId();
            var result = await _friendSidebarService.GetOnlineFriendsAsync(userId);
            return Ok(result);
        }

        [HttpGet("friend-sidebar")]
        public async Task<IActionResult> GetFriendSidebar()
        {
            var userId = GetCurrentUserId();
            var result = await _friendSidebarService.GetFriendSidebarAsync(userId);
            return Ok(result);
        }
    }
}