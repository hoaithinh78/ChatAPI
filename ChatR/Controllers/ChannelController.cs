using ChatR.DTOs.Channels;
using ChatR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelController : ControllerBase
    {
        private readonly ChannelService _channelService;

        public ChannelController(ChannelService channelService)
        {
            _channelService = channelService;
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

        [HttpGet("server/{serverId}")]
        public async Task<IActionResult> GetChannelsByServerId(int serverId)
        {
            var channels = await _channelService.GetChannelsByServerIdAsync(serverId);
            return Ok(channels);
        }
    }
}
