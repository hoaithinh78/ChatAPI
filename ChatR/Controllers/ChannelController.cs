using ChatR.DTOs.Channels;
using ChatR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private readonly ChannelService _channelService;

        public ChannelController(ChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpGet("server/{serverId}")]
        public async Task<IActionResult> GetChannelsByServerId(int serverId)
        {
            var channels = await _channelService.GetChannelsByServerIdAsync(serverId);
            return Ok(channels);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelRequest request)
        {
            try
            {
                var result = await _channelService.CreateChannelAsync(request);
                return Ok(new
                {
                    message = "Tạo channel thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
