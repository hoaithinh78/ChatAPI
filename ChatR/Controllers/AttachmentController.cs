using System.Security.Claims;
using ChatR.Data;
using ChatR.DTOs.Attachment;
using ChatR.Helpers;
using ChatR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AttachmentController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                throw new Exception("Không lấy được userId từ token.");
            return userId;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Upload([FromForm] UploadAttachmentDto dto, CancellationToken cancellationToken)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest(new { message = "File không hợp lệ." });

            var channelExists = await _dbContext.Channels
                .AnyAsync(c => c.ChannelId == dto.ChannelId, cancellationToken);

            if (!channelExists)
                return BadRequest(new { message = "Channel không tồn tại." });

            var senderId = GetCurrentUserId();

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chat");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(dto.File.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream, cancellationToken);
            }

            var relativeUrl = $"/uploads/chat/{fileName}";

            var message = new Message
            {
                SenderId = senderId,
                Content = CryptoHelper.Encrypt(dto.Content ?? ""),
                CreatedAt = DateTime.UtcNow,
                EditedAt = null,
                IsDeleted = 0,
                ChannelId = dto.ChannelId,
                ConversationId = null
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var attachment = new Attachment
            {
                MessageId = message.MessageId,
                FileUrl = relativeUrl,
                FileType = dto.File.ContentType,
                FileSize = dto.File.Length
            };

            _dbContext.Attachments.Add(attachment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var sender = await _dbContext.Users
                .Where(u => u.UserId == senderId)
                .Select(u => new
                {
                    DisplayName = u.DisplayName ?? u.Username ?? $"User {u.UserId}",
                    u.AvatarUrl
                })
                .FirstOrDefaultAsync(cancellationToken);

            return Ok(new
            {
                message.MessageId,
                message.SenderId,
                SenderName = sender?.DisplayName,
                AvatarUrl = sender?.AvatarUrl,
                Content = dto.Content ?? "",
                message.CreatedAt,
                message.ChannelId,
                Attachments = new[]
                {
                    new
                    {
                        attachment.AttachId,
                        attachment.FileUrl,
                        attachment.FileType,
                        attachment.FileSize
                    }
                }
            });
        }
    }
}