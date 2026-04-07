using ChatR.Data;
using ChatR.DTOs.Conversation;
using ChatR.DTOs.Message;
using ChatR.Models;
using ChatR.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly AppDbContext _dbContext;

        public MessageController(IMessageService messageService, AppDbContext dbContext)
        {
            _messageService = messageService;
            _dbContext = dbContext;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                throw new Exception("Không lấy được userId từ token.");

            return userId;
        }
        [HttpGet("channel/{channelId}")]
        public async Task<IActionResult> GetMessagesByChannel(int channelId, CancellationToken cancellationToken)
        {
            var result = await _messageService.GetMessagesByChannelAsync(channelId, cancellationToken);
            return Ok(result);
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
        {
            var senderId = GetCurrentUserId();
            var result = await _messageService.SendMessageAsync(senderId, dto, cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-messages/conversation/{conversationId}")]
        public async Task<IActionResult> GetMessagesByConversation(int conversationId, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var isMember = await _dbContext.ConversationMembers
                .AnyAsync(x => x.ConversationId == conversationId && x.UserId == currentUserId, cancellationToken);

            if (!isMember)
                return Forbid();

            var result = await _messageService.GetMessagesByConversationAsync(conversationId, cancellationToken);
            return Ok(result);
        }

        [HttpPut("edit-message")]
        public async Task<IActionResult> EditMessage([FromBody] EditMessageDto dto, CancellationToken cancellationToken)
        {
            var senderId = GetCurrentUserId();
            var result = await _messageService.EditMessageAsync(senderId, dto, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("delete-message/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId, CancellationToken cancellationToken)
        {
            var senderId = GetCurrentUserId();
            await _messageService.DeleteMessageAsync(senderId, messageId, cancellationToken);
            return Ok("Tin nhắn đã được xóa.");
        }

        [HttpPost("read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId && m.IsDeleted == 0, cancellationToken);

            if (message == null)
                return NotFound("Tin nhắn không tồn tại.");

            if (message.ConversationId.HasValue)
            {
                var isMember = await _dbContext.ConversationMembers
                    .AnyAsync(x => x.ConversationId == message.ConversationId.Value && x.UserId == currentUserId, cancellationToken);

                if (!isMember)
                    return Forbid();
            }

            var existed = await _dbContext.MessageReads
                .AnyAsync(x => x.MessageId == messageId && x.UserId == currentUserId, cancellationToken);

            if (!existed)
            {
                _dbContext.MessageReads.Add(new MessageRead
                {
                    MessageId = messageId,
                    UserId = currentUserId,
                    ReadAt = DateTime.UtcNow
                });

                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Ok("Đã đánh dấu tin nhắn là đã đọc.");
        }

    }
}