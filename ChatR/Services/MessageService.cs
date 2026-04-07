using ChatR.Evens;
using ChatR.Interface;
using ChatR.Data;
using ChatR.DTOs;
using ChatR.Helpers;
using ChatR.Models;
using ChatR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;

        public MessageService(AppDbContext dbContext, IEventPublisher eventPublisher)
        {
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
        }

        public async Task<object> SendMessageAsync(long senderId, SendMessageDto dto, CancellationToken cancellationToken = default)
        {
            dto.Content ??= "";
            dto.ChannelId ??= 0;
            dto.ConversationId ??= 0;

            Conversation? conversation = null;

            if (dto.ConversationId > 0)
            {
                conversation = await _dbContext.Conversations
                    .Include(c => c.ConversationMembers)
                    .FirstOrDefaultAsync(c => c.ConversationId == dto.ConversationId, cancellationToken);
            }
            else if (dto.ReceiverId.HasValue)
            {
                conversation = await _dbContext.Conversations
                    .Include(c => c.ConversationMembers)
                    .FirstOrDefaultAsync(c =>
                        c.Type == 1 &&
                        c.ConversationMembers.Any(cm => cm.UserId == senderId) &&
                        c.ConversationMembers.Any(cm => cm.UserId == dto.ReceiverId.Value),
                        cancellationToken);

                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        Type = 1,
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.Conversations.Add(conversation);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _dbContext.ConversationMembers.AddRange(new[]
                    {
                        new ConversationMember { ConversationId = conversation.ConversationId, UserId = (int)senderId },
                        new ConversationMember { ConversationId = conversation.ConversationId, UserId = dto.ReceiverId.Value }
                    });

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            int? conversationId = conversation?.ConversationId;

            var message = new Message
            {
                SenderId = (int)senderId,
                Content = CryptoHelper.Encrypt(dto.Content),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = 0,
                ChannelId = dto.ChannelId > 0 ? dto.ChannelId : null,
                ConversationId = conversationId
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishAsync(
                new MessageCreatedEvent(
                    message.MessageId,
                    senderId,
                    message.ChannelId,
                    message.ConversationId,
                    dto.Content),
                cancellationToken);

            return new
            {
                message.MessageId,
                message.SenderId,
                Content = dto.Content,
                message.CreatedAt,
                message.ChannelId,
                message.ConversationId
            };
        }

        public async Task<object> GetMessagesByConversationAsync(int conversationId, CancellationToken cancellationToken = default)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId && m.IsDeleted == 0)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);

            return messages.Select(m => new
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
        }

        public async Task<object> EditMessageAsync(long senderId, EditMessageDto dto, CancellationToken cancellationToken = default)
        {
            dto.Content ??= "";

            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == dto.MessageId && m.SenderId == senderId, cancellationToken);

            if (message == null)
                throw new Exception("Tin nhắn không tồn tại hoặc không có quyền chỉnh sửa.");

            message.Content = CryptoHelper.Encrypt(dto.Content);
            message.EditedAt = DateTime.UtcNow;

            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new
            {
                message.MessageId,
                message.SenderId,
                Content = dto.Content,
                message.CreatedAt,
                message.EditedAt,
                message.ChannelId,
                message.ConversationId
            };
        }

        public async Task DeleteMessageAsync(long senderId, int messageId, CancellationToken cancellationToken = default)
        {
            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId && m.SenderId == senderId, cancellationToken);

            if (message == null)
                throw new Exception("Tin nhắn không tồn tại hoặc không có quyền xóa.");

            message.IsDeleted = 1;
            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync(cancellationToken);
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
                return cipher;
            }
        }
    }
}
