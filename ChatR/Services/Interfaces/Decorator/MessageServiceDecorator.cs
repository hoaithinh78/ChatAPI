using ChatR.DTOs.Conversation;
using ChatR.DTOs.Message;


namespace ChatR.Services.Interfaces.Decorator
{
    public abstract class MessageServiceDecorator : IMessageServiceDecorator
    {
        protected readonly IMessageServiceDecorator _inner;

        public MessageServiceDecorator(IMessageServiceDecorator inner)
        {
            _inner = inner;
        }

        public virtual Task<object> SendMessageAsync(int senderId, SendMessageDto dto, CancellationToken cancellationToken = default)
            => _inner.SendMessageAsync(senderId, dto, cancellationToken);

        public virtual Task<object> GetMessagesByConversationAsync(int conversationId, CancellationToken cancellationToken = default)
            => _inner.GetMessagesByConversationAsync(conversationId, cancellationToken);

        public virtual Task<object> GetMessagesByChannelAsync(int channelId, CancellationToken cancellationToken = default)
            => _inner.GetMessagesByChannelAsync(channelId, cancellationToken);

        public virtual Task<object> EditMessageAsync(int senderId, EditMessageDto dto, CancellationToken cancellationToken = default)
            => _inner.EditMessageAsync(senderId, dto, cancellationToken);

        public virtual Task DeleteMessageAsync(int senderId, int messageId, CancellationToken cancellationToken = default)
            => _inner.DeleteMessageAsync(senderId, messageId, cancellationToken);
    }
}
