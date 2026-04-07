using ChatR.Data;
using ChatR.DTOs.Conversation;
using ChatR.Helpers;
using ChatR.Hubs;
using ChatR.Models;
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
    public class ConversationController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;

        public ConversationController(
            AppDbContext dbContext,
            IHubContext<ChatHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                throw new Exception("Không lấy được userId từ token.");

            return userId;
        }

        private string SafeDecrypt(string cipher)
        {
            if (string.IsNullOrWhiteSpace(cipher))
                return "";

            try
            {
                return CryptoHelper.Decrypt(cipher);
            }
            catch
            {
                return cipher;
            }
        }

        [HttpGet("my-conversations")]
        public async Task<IActionResult> GetMyConversations()
        {
            var currentUserId = GetCurrentUserId();

            var conversations = await _dbContext.ConversationMembers
                .Where(cm => cm.UserId == currentUserId)
                .Select(cm => cm.Conversation)
                .Select(c => new
                {
                    c.ConversationId,
                    c.Type,
                    c.Name,
                    c.CreatedAt
                })
                .ToListAsync();

            if (!conversations.Any())
                return Ok(new List<object>());

            var conversationIds = conversations
                .Select(c => c.ConversationId)
                .ToList();

            var membersRaw = await _dbContext.ConversationMembers
                .Where(cm => conversationIds.Contains(cm.ConversationId))
                .Join(_dbContext.Users,
                    cm => cm.UserId,
                    u => u.UserId,
                    (cm, u) => new
                    {
                        cm.ConversationId,
                        u.UserId,
                        DisplayName = u.DisplayName ?? u.Username ?? u.Email,
                        u.AvatarUrl,
                        u.Status
                    })
                .ToListAsync();

            var messagesRaw = await _dbContext.Messages
                .Where(m => m.ConversationId.HasValue
                            && conversationIds.Contains(m.ConversationId.Value)
                            && m.IsDeleted == 0)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new
                {
                    m.MessageId,
                    m.SenderId,
                    m.Content,
                    m.CreatedAt,
                    m.EditedAt,
                    m.ConversationId
                })
                .ToListAsync();

            var lastMessages = messagesRaw
                .GroupBy(m => m.ConversationId!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var allMessages = await _dbContext.Messages
                .Where(m => m.ConversationId.HasValue
                            && conversationIds.Contains(m.ConversationId.Value)
                            && m.IsDeleted == 0)
                .Select(m => new
                {
                    m.MessageId,
                    m.ConversationId,
                    m.SenderId
                })
                .ToListAsync();

            var messageIds = allMessages.Select(m => m.MessageId).ToList();

            var readMessageIds = await _dbContext.MessageReads
                .Where(r => r.UserId == currentUserId && messageIds.Contains(r.MessageId))
                .Select(r => r.MessageId)
                .ToListAsync();

            var readMessageIdSet = readMessageIds.ToHashSet();

            var result = conversations
                .Select(c =>
                {
                    var members = membersRaw
                        .Where(m => m.ConversationId == c.ConversationId)
                        .Select(m => new
                        {
                            m.UserId,
                            m.DisplayName,
                            m.AvatarUrl,
                            m.Status
                        })
                        .ToList();

                    lastMessages.TryGetValue(c.ConversationId, out var lastMessage);

                    var unreadCount = allMessages.Count(m =>
                        m.ConversationId == c.ConversationId &&
                        m.SenderId != currentUserId &&
                        !readMessageIdSet.Contains(m.MessageId));

                    return new
                    {
                        c.ConversationId,
                        c.Type,
                        c.Name,
                        c.CreatedAt,
                        Members = members,
                        LastMessage = lastMessage == null ? null : new
                        {
                            lastMessage.MessageId,
                            lastMessage.SenderId,
                            Content = SafeDecrypt(lastMessage.Content),
                            lastMessage.CreatedAt,
                            lastMessage.EditedAt
                        },
                        UnreadCount = unreadCount,
                        SortTime = lastMessage?.CreatedAt ?? c.CreatedAt
                    };
                })
                .OrderByDescending(x => x.SortTime)
                .Select(x => new
                {
                    x.ConversationId,
                    x.Type,
                    x.Name,
                    x.CreatedAt,
                    x.Members,
                    x.LastMessage,
                    x.UnreadCount
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationDetail(int conversationId)
        {
            var currentUserId = GetCurrentUserId();

            var isMember = await _dbContext.ConversationMembers
                .AnyAsync(x => x.ConversationId == conversationId && x.UserId == currentUserId);

            if (!isMember)
                return Forbid();

            var conversation = await _dbContext.Conversations
                .Where(c => c.ConversationId == conversationId)
                .Select(c => new
                {
                    c.ConversationId,
                    c.Type,
                    c.Name,
                    c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (conversation == null)
                return NotFound("Không tìm thấy cuộc trò chuyện.");

            return Ok(conversation);
        }

        [HttpGet("members/{conversationId}")]
        public async Task<IActionResult> GetConversationMembers(int conversationId)
        {
            var currentUserId = GetCurrentUserId();

            var isMember = await _dbContext.ConversationMembers
                .AnyAsync(x => x.ConversationId == conversationId && x.UserId == currentUserId);

            if (!isMember)
                return Forbid();

            var members = await _dbContext.ConversationMembers
                .Where(cm => cm.ConversationId == conversationId)
                .Join(_dbContext.Users,
                    cm => cm.UserId,
                    u => u.UserId,
                    (cm, u) => new
                    {
                        u.UserId,
                        DisplayName = u.DisplayName ?? u.Username ?? u.Email,
                        u.AvatarUrl,
                        u.Status
                    })
                .ToListAsync();

            return Ok(members);
        }

        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var conversation = new Conversation
            {
                Type = dto.Type,
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Conversations.Add(conversation);
            await _dbContext.SaveChangesAsync();

            var members = new List<ConversationMember>
            {
                new ConversationMember
                {
                    ConversationId = conversation.ConversationId,
                    UserId = currentUserId
                }
            };

            if (dto.UserIds != null && dto.UserIds.Any())
            {
                var distinctUserIds = dto.UserIds
                    .Where(x => x > 0 && x != currentUserId)
                    .Distinct()
                    .ToList();

                foreach (var userId in distinctUserIds)
                {
                    var userExists = await _dbContext.Users.AnyAsync(x => x.UserId == userId);
                    if (!userExists) continue;

                    members.Add(new ConversationMember
                    {
                        ConversationId = conversation.ConversationId,
                        UserId = userId
                    });
                }
            }

            _dbContext.ConversationMembers.AddRange(members);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                conversation.ConversationId,
                conversation.Type,
                conversation.Name,
                conversation.CreatedAt
            });
        }

        [HttpPost("join-conversation")]
        public async Task<IActionResult> JoinConversation([FromBody] JoinConversationDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var conversationExists = await _dbContext.Conversations
                .AnyAsync(c => c.ConversationId == dto.ConversationId);

            if (!conversationExists)
                return NotFound("Cuộc trò chuyện không tồn tại.");

            var alreadyJoined = await _dbContext.ConversationMembers
                .AnyAsync(x => x.ConversationId == dto.ConversationId && x.UserId == currentUserId);

            if (alreadyJoined)
                return BadRequest("Bạn đã là thành viên của cuộc trò chuyện.");

            var member = new ConversationMember
            {
                ConversationId = dto.ConversationId,
                UserId = currentUserId
            };

            _dbContext.ConversationMembers.Add(member);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.Group(dto.ConversationId.ToString())
                .SendAsync("UserJoined", currentUserId);

            return Ok("Bạn đã tham gia cuộc trò chuyện.");
        }

        [HttpPost("leave-conversation")]
        public async Task<IActionResult> LeaveConversation([FromBody] LeaveConversationDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var member = await _dbContext.ConversationMembers
                .FirstOrDefaultAsync(m => m.ConversationId == dto.ConversationId && m.UserId == currentUserId);

            if (member == null)
                return NotFound("Bạn không phải là thành viên của cuộc trò chuyện.");

            _dbContext.ConversationMembers.Remove(member);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.Group(dto.ConversationId.ToString())
                .SendAsync("UserLeft", currentUserId);

            return Ok("Bạn đã rời cuộc trò chuyện.");
        }

        [HttpPut("{conversationId}/read-all")]
        public async Task<IActionResult> ReadAllMessages(int conversationId)
        {
            var currentUserId = GetCurrentUserId();

            var isMember = await _dbContext.ConversationMembers
                .AnyAsync(x => x.ConversationId == conversationId && x.UserId == currentUserId);

            if (!isMember)
                return Forbid();

            var unreadMessages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId
                            && m.SenderId != currentUserId
                            && m.IsDeleted == 0
                            && !_dbContext.MessageReads.Any(r => r.MessageId == m.MessageId && r.UserId == currentUserId))
                .ToListAsync();

            if (unreadMessages.Any())
            {
                var reads = unreadMessages.Select(m => new MessageRead
                {
                    MessageId = m.MessageId,
                    UserId = currentUserId,
                    ReadAt = DateTime.UtcNow
                }).ToList();

                _dbContext.MessageReads.AddRange(reads);
                await _dbContext.SaveChangesAsync();
            }

            return Ok("Đã đánh dấu tất cả tin nhắn là đã đọc.");
        }
    }
}