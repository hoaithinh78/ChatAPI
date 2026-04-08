using ChatR.Data;
using ChatR.DTOs.Channels;
using ChatR.DTOs.Server;
using ChatR.Models;
using ChatR.Services.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServerController : ControllerBase
    {
        private readonly ServerService _serverService;
        private readonly ChannelService _channelService;
        private readonly AppDbContext _dbContext;
        public ServerController(ServerService serverService, ChannelService channelService, AppDbContext dbContext)
        {
            _serverService = serverService;
            _channelService = channelService;
            _dbContext = dbContext;
        }
        [HttpGet("{serverId}")]
        public async Task<IActionResult> GetServerById(int serverId)
        {
            var server = await _dbContext.Servers
                .Where(s => s.ServerId == serverId)
                .Select(s => new
                {
                    s.ServerId,
                    s.ServerName,
                    s.Description,
                    s.IconUrl,
                    s.OwnerId,
                    s.TotalMembers,
                    s.OnlineMembers,
                    s.Score,
                    s.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (server == null)
                return NotFound(new { message = "Không tìm thấy server." });

            return Ok(server);
        }
        [HttpGet("get-server-members/{serverId}")]
        public async Task<IActionResult> GetServerMembers(int serverId)
        {
            var members = await _serverService.GetServerMembersAsync(serverId);
            return Ok(members);
        }

        [HttpPost("create-server")]
        public async Task<IActionResult> CreateServer([FromBody] CreateServerDto createServerDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _serverService.CreateServerAsync(userId, createServerDto);
            return Ok(result);
        }

        [HttpPost("add-member")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberDto addMemberDto)
        {
            var result = await _serverService.AddMemberAsync(addMemberDto);
            return Ok(result);
        }

        [HttpPost("create-channel")]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelRequest request)
        {
            var result = await _channelService.CreateChannelAsync(request);
            return Ok(result);
        }
    }
}