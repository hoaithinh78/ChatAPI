using ChatR.Server.Data;
using ChatR.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatR.Server.Hubs;
using ChatR.Server.DTOs;
namespace ChatR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {

        private readonly AppDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        public ConversationController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("get-conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var conversations = await _dbContext.Conversations.ToListAsync();

            if (conversations == null || !conversations.Any())
            {
                return NotFound("Không có cuộc trò chuyện nào.");
            }

            return Ok(conversations);
        }


        [HttpGet("get-conversation-members/{conversationId}")]
        public async Task<IActionResult> GetConversationMembers(int conversationId)
        {
            var members = await _dbContext.ConversationMembers
                .Where(cm => cm.ConversationId == conversationId)
                .ToListAsync();

            if (members == null || !members.Any())
            {
                return NotFound("Không có thành viên nào trong cuộc trò chuyện này.");
            }

            return Ok(members);
        }
        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto createConversationDto)
        {
            var conversation = new Conversation
            {
                Type = createConversationDto.Type, // 1: Public, 2: Private
                Name = createConversationDto.Name,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Conversations.Add(conversation);
            await _dbContext.SaveChangesAsync();

            return Ok(conversation);
        }

        [HttpPost("join-conversation")]
        public async Task<IActionResult> JoinConversation([FromBody] JoinConversationDto joinConversationDto)
        {
            var member = new ConversationMember
            {
                ConversationId = joinConversationDto.ConversationId,
                UserId = joinConversationDto.UserId
            };

            _dbContext.ConversationMembers.Add(member);
            await _dbContext.SaveChangesAsync();

            // Tham gia nhóm SignalR
            await _hubContext.Clients.Group(joinConversationDto.ConversationId.ToString()).SendAsync("UserJoined", joinConversationDto.UserId);

            return Ok("Bạn đã tham gia cuộc trò chuyện.");
        }

        [HttpPost("leave-conversation")]
        public async Task<IActionResult> LeaveConversation([FromBody] LeaveConversationDto leaveConversationDto)
        {
            var member = await _dbContext.ConversationMembers
                .FirstOrDefaultAsync(m => m.ConversationId == leaveConversationDto.ConversationId && m.UserId == leaveConversationDto.UserId);

            if (member == null)
            {
                return NotFound("Bạn không phải là thành viên của cuộc trò chuyện.");
            }

            _dbContext.ConversationMembers.Remove(member);
            await _dbContext.SaveChangesAsync();

            // Rời nhóm SignalR
            await _hubContext.Clients.Group(leaveConversationDto.ConversationId.ToString()).SendAsync("UserLeft", leaveConversationDto.UserId);

            return Ok("Bạn đã rời cuộc trò chuyện.");
        }

        
    }
}
