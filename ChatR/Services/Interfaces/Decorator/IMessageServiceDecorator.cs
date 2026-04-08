using ChatR.DTOs.Conversation;
using ChatR.DTOs.Message;

namespace ChatR.Services.Interfaces.Decorator
{
    public interface IMessageServiceDecorator
    {
        Task<object> SendMessageAsync(int senderId, SendMessageDto dto, CancellationToken cancellationToken = default);
        Task<object> GetMessagesByConversationAsync(int conversationId, CancellationToken cancellationToken = default);
        Task<object> GetMessagesByChannelAsync(int channelId, CancellationToken cancellationToken = default);
        Task<object> EditMessageAsync(int senderId, EditMessageDto dto, CancellationToken cancellationToken = default);
        Task DeleteMessageAsync(int senderId, int messageId, CancellationToken cancellationToken = default);
    }
}
