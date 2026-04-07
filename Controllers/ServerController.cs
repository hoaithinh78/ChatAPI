using ChatR.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChatR.Server.DTOs;
using ChatR.Server.Data;

namespace ChatR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public ServerController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("get-server-members/{serverId}")]
        public async Task<IActionResult> GetServerMembers(int serverId)
        {
            var members = await _dbContext.ServerMembers
                .Where(sm => sm.ServerId == serverId)
                .ToListAsync();

            if (members == null || !members.Any())
            {
                return NotFound("Không có thành viên nào trong server này.");
            }

            return Ok(members);
        }

        [HttpPost("create-server")]
        public async Task<IActionResult> CreateServer([FromBody] CreateServerDto createServerDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var server = new Servers
            {
                ServerName = createServerDto.ServerName,
                OwnerId = userId,
                Description = createServerDto.Description,
                CreatedAt = DateTime.UtcNow,
                TotalMembers = 1,  
                OnlineMembers = 0,
                Score = 0
            };

            _dbContext.Servers.Add(server);
            await _dbContext.SaveChangesAsync();

            return Ok(server);
        }

        [HttpPost("add-member")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberDto addMemberDto)
        {
            var server = await _dbContext.Servers
                .FirstOrDefaultAsync(s => s.ServerId == addMemberDto.ServerId);

            if (server == null)
            {
                return NotFound("Server không tồn tại.");
            }

            var member = new ServerMember
            {
                ServerId = addMemberDto.ServerId,
                UserId = addMemberDto.UserId,
                JoinedAt = DateTime.UtcNow,
                IsOnline = 0  
            };

            _dbContext.ServerMembers.Add(member);
            await _dbContext.SaveChangesAsync();

            return Ok("Thành viên đã được thêm vào server.");
        }

        [HttpPost("create-channel")]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelDto createChannelDto)
        {
            var channel = new Channel
            {
                ChannelName = createChannelDto.ChannelName,
                Type = createChannelDto.Type,
                CreatedAt = DateTime.UtcNow,
                ServerId = createChannelDto.ServerId
            };

            _dbContext.Channels.Add(channel);
            await _dbContext.SaveChangesAsync();

            return Ok(channel);
        }
        
    }
}
