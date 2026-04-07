using ChatR.DTOs;

namespace ChatR.Services.Interfaces
{
    public interface IMessageService
    {
        Task<object> SendMessageAsync(long senderId, SendMessageDto dto, CancellationToken cancellationToken = default);
        Task<object> GetMessagesByConversationAsync(int conversationId, CancellationToken cancellationToken = default);
        Task<object> EditMessageAsync(long senderId, EditMessageDto dto, CancellationToken cancellationToken = default);
        Task DeleteMessageAsync(long senderId, int messageId, CancellationToken cancellationToken = default);
    }
}
