using ChatR.Server.Data;
using ChatR.Server.DTOs;
using ChatR.Server.Helpers;
using ChatR.Server.Hubs;
using ChatR.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AppDbContext _dbContext;

        public MessageController(AppDbContext dbContext, IHubContext<ChatHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        // Lấy danh sách tin nhắn trong conversation
        [HttpGet("get-messages/conversation/{conversationId}")]
        public async Task<IActionResult> GetMessagesByConversation(int conversationId)
        {
            var messages = await _dbContext.Messages
            .Where(m => m.ConversationId == conversationId && m.IsDeleted == 0)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

            var result = messages.Select(m => new
            {
                m.MessageId,
                m.SenderId,
                Content = SafeDecrypt(m.Content),
                m.CreatedAt,
                m.EditedAt,
                m.IsDeleted,
                m.ChannelId,
                m.ConversationId
            });

            return Ok(result);
        }
        private string SafeDecrypt(string cipher)
        {
            if (string.IsNullOrEmpty(cipher))
                return "";

            try
            {
                return CryptoHelper.Decrypt(cipher);
            }
            catch
            {
                // Nếu giải mã lỗi thì trả về nội dung thô
                return cipher;
            }
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            // Lấy userId từ JWT token
            var senderIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(senderIdString, out int senderId))
                return Unauthorized("Không lấy được userId từ token.");

            dto.Content ??= "";     
            dto.ChannelId ??= 0;   
            dto.ConversationId ??= 0;

            Conversation conversation = null;

            if (dto.ConversationId > 0)
            {
                // Group chat
                conversation = await _dbContext.Conversations
                    .Include(c => c.ConversationMembers)
                    .FirstOrDefaultAsync(c => c.ConversationId == dto.ConversationId);
            }
            else if (dto.ReceiverId.HasValue)
            {
                // Private chat
                conversation = await _dbContext.Conversations
                    .Include(c => c.ConversationMembers)
                    .FirstOrDefaultAsync(c =>
                        c.Type == 1 &&
                        c.ConversationMembers.Any(cm => cm.UserId == senderId) &&
                        c.ConversationMembers.Any(cm => cm.UserId == dto.ReceiverId.Value)
                    );

                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        Type = 1,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.Conversations.Add(conversation);
                    await _dbContext.SaveChangesAsync();

                    _dbContext.ConversationMembers.AddRange(new[]
                    {
                new ConversationMember { ConversationId = conversation.ConversationId, UserId = senderId },
                new ConversationMember { ConversationId = conversation.ConversationId, UserId = dto.ReceiverId.Value }
            });
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Nếu vẫn null => conversationId = null
            int? conversationId = conversation?.ConversationId;

            var message = new Message
            {
                SenderId = senderId,
                Content = CryptoHelper.Encrypt(dto.Content ?? ""),  // mã hóa
                CreatedAt = DateTime.UtcNow,
                IsDeleted = 0,
                ChannelId = dto.ChannelId > 0 ? dto.ChannelId : null,
                ConversationId = conversationId
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            if (conversationId.HasValue)
            {
                await _hubContext.Clients.Group(conversationId.Value.ToString())
                .SendAsync("ReceiveMessage", senderId, CryptoHelper.Encrypt(dto.Content));
            }

            return Ok(message);
        }
        [HttpPut("edit-message")]
        public async Task<IActionResult> EditMessage([FromBody] EditMessageDto dto)
        {
            var senderIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(senderIdString, out int senderId))
                return Unauthorized();

            dto.Content ??= "";

            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == dto.MessageId && m.SenderId == senderId);

            if (message == null)
                return NotFound("Tin nhắn không tồn tại hoặc không có quyền chỉnh sửa.");

            message.Content = dto.Content;
            message.EditedAt = DateTime.UtcNow;

            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync();

            if (message.ConversationId.HasValue)
            {
                await _hubContext.Clients.Group(message.ConversationId.Value.ToString())
                    .SendAsync("ReceiveMessage", senderId, dto.Content);
            }

            return Ok(message);
        }

        [HttpDelete("delete-message/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var senderIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(senderIdString, out int senderId))
                return Unauthorized();

            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId && m.SenderId == senderId);

            if (message == null)
                return NotFound("Tin nhắn không tồn tại hoặc không có quyền xóa.");

            message.IsDeleted = 1;
            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync();

            return Ok("Tin nhắn đã được xóa.");
        }
    }
}