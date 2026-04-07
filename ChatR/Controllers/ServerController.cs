using ChatR.Data;
using ChatR.DTOs;
using ChatR.DTOs.Channels;
using ChatR.DTOs.Server;
using ChatR.Models;
using ChatR.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly ServerService _serverService;
        private readonly ChannelService _channelService;

        public ServerController(ServerService serverService, ChannelService channelService)
        {
            _serverService = serverService;
            _channelService = channelService;
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