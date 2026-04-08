using ChatR.Data;
using ChatR.DTOs.Conversation;
using ChatR.Models;

namespace ChatR.Services.Interfaces.Decorator
{
    public class MessageAuditDecorator : MessageServiceDecorator
    {
        private readonly AppDbContext _db;

        public MessageAuditDecorator(IMessageServiceDecorator inner, AppDbContext db)
            : base(inner)
        {
            _db = db;
        }

        public override async Task<object> SendMessageAsync(int senderId, SendMessageDto dto, CancellationToken cancellationToken = default)
        {
            var result = await base.SendMessageAsync(senderId, dto, cancellationToken);

            _db.AuditLogs.Add(new AuditLog
            {
                Action = "SEND_MESSAGE",
                TargetType = "MESSAGE",
                NewValue = dto.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = senderId
            });

            await _db.SaveChangesAsync();

            return result;
        }
    }
}
