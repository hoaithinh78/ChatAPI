using ChatR.Server.DTOs;
using ChatR.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatR.Server.Data;

namespace ChatR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageReadController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public MessageReadController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("get-messages/conversation/{conversationId}")]
        public async Task<IActionResult> GetMessagesByConversation(int conversationId)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId && m.IsDeleted == 0)  // Chỉ lấy tin nhắn chưa bị xóa
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            if (messages == null || !messages.Any())
            {
                return NotFound("Không có tin nhắn nào trong cuộc trò chuyện này.");
            }

            return Ok(messages);
        }

        [HttpPost("message-read")]
        public async Task<IActionResult> MarkMessageAsRead([FromBody] MarkMessageReadDto markMessageReadDto)
        {
            var messageRead = new MessageRead
            {
                MessageId = markMessageReadDto.MessageId,
                UserId = markMessageReadDto.UserId,
                ReadAt = DateTime.UtcNow
            };

            _dbContext.MessageReads.Add(messageRead);
            await _dbContext.SaveChangesAsync();

            return Ok("Tin nhắn đã được đánh dấu là đã đọc.");
        }

    }
}
