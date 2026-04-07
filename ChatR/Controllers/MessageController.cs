using ChatR.Data;
using ChatR.DTOs;
using ChatR.Helpers;
using ChatR.Hubs;
using ChatR.Models;
using ChatR.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
        {
            var senderIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(senderIdString, out var senderId))
                return Unauthorized("Không lấy được userId từ token.");

            var result = await _messageService.SendMessageAsync(senderId, dto, cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-messages/conversation/{conversationId}")]
        public async Task<IActionResult> GetMessagesByConversation(int conversationId, CancellationToken cancellationToken)
        {
            var result = await _messageService.GetMessagesByConversationAsync(conversationId, cancellationToken);
            return Ok(result);
        }

        [HttpPut("edit-message")]
        public async Task<IActionResult> EditMessage([FromBody] EditMessageDto dto, CancellationToken cancellationToken)
        {
            var senderIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(senderIdString, out var senderId))
                return Unauthorized();

            var result = await _messageService.EditMessageAsync(senderId, dto, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("delete-message/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId, CancellationToken cancellationToken)
        {
            var senderIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(senderIdString, out var senderId))
                return Unauthorized();

            await _messageService.DeleteMessageAsync(senderId, messageId, cancellationToken);
            return Ok("Tin nhắn đã được xóa.");
        }
    }
}