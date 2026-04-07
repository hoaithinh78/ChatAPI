using ChatR.Evens;
using ChatR.Interface;
using ChatR.Data;
using ChatR.Helpers;
using ChatR.Models;
using ChatR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChatR.DTOs.Message;
using ChatR.DTOs.Conversation;

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

        public async Task<object> SendMessageAsync(int senderId, SendMessageDto dto, CancellationToken cancellationToken = default)
        {
            dto.Content ??= "";

            Conversation? conversation = null;

            // 🔥 PRIVATE CHAT
            if (dto.ReceiverId.HasValue)
            {
                conversation = await _dbContext.Conversations
                    .Where(c => c.Type == 1)
                    .Where(c => c.ConversationMembers.Any(cm => cm.UserId == senderId))
                    .Where(c => c.ConversationMembers.Any(cm => cm.UserId == dto.ReceiverId.Value))
                    .FirstOrDefaultAsync(cancellationToken);

                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        Type = 1,
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.Conversations.Add(conversation);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    var members = new List<ConversationMember>
                    {
                        new ConversationMember
                        {
                            ConversationId = conversation.ConversationId,
                            UserId = senderId
                        },
                        new ConversationMember
                        {
                            ConversationId = conversation.ConversationId,
                            UserId = dto.ReceiverId.Value
                        }
                    };

                    _dbContext.ConversationMembers.AddRange(members);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            // ❗ VALIDATE
            if (conversation == null && (!dto.ChannelId.HasValue || dto.ChannelId <= 0))
            {
                throw new Exception("Phải có receiverId hoặc channelId");
            }

            // 🔥 CREATE MESSAGE
            var message = new Message
            {
                SenderId = senderId,
                Content = CryptoHelper.Encrypt(dto.Content),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = 0,
                ChannelId = dto.ChannelId > 0 ? dto.ChannelId : null,
                ConversationId = conversation?.ConversationId
            };

            _dbContext.Messages.Add(message);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }

            // 🔥 EVENT
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

        // 🔥 FIX: long → int
        public async Task<object> EditMessageAsync(int senderId, EditMessageDto dto, CancellationToken cancellationToken = default)
        {
            dto.Content ??= "";

            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == dto.MessageId && m.SenderId == senderId, cancellationToken);

            if (message == null)
                throw new Exception("Tin nhắn không tồn tại hoặc không có quyền chỉnh sửa.");

            message.Content = CryptoHelper.Encrypt(dto.Content);
            message.EditedAt = DateTime.UtcNow;

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

        // 🔥 FIX: long → int
        public async Task DeleteMessageAsync(int senderId, int messageId, CancellationToken cancellationToken = default)
        {
            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId && m.SenderId == senderId, cancellationToken);

            if (message == null)
                throw new Exception("Tin nhắn không tồn tại hoặc không có quyền xóa.");

            message.IsDeleted = 1;
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