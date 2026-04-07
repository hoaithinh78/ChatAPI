using ChatR.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new Exception("Không lấy được userId từ token.");

            return int.Parse(userIdClaim);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetSummaryAsync(userId);
            return Ok(result);
        }

        [HttpGet("my-servers")]
        public async Task<IActionResult> GetMyServers()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetMyServersAsync(userId);
            return Ok(result);
        }

        [HttpGet("recent-servers")]
        public async Task<IActionResult> GetRecentServers()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetRecentServersAsync(userId);
            return Ok(result);
        }

        [HttpGet("direct-messages")]
        public async Task<IActionResult> GetDirectMessages()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetDirectMessagesAsync(userId);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetCurrentUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("unread-notifications-count")]
        public async Task<IActionResult> GetUnreadNotificationsCount()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetUnreadNotificationCountAsync(userId);
            return Ok(new { unreadCount = result });
        }
    }
}
