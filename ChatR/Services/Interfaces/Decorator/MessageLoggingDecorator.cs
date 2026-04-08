using ChatR.DTOs.Conversation;

namespace ChatR.Services.Interfaces.Decorator
{
    public class MessageLoggingDecorator : MessageServiceDecorator
    {
        private readonly ILogger<MessageLoggingDecorator> _logger;

        public MessageLoggingDecorator(IMessageServiceDecorator inner, ILogger<MessageLoggingDecorator> logger)
            : base(inner)
        {
            _logger = logger;
        }

        public override async Task<object> SendMessageAsync(int senderId, SendMessageDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"[SEND] User {senderId}");

            var result = await base.SendMessageAsync(senderId, dto, cancellationToken);

            _logger.LogInformation($"[DONE] User {senderId}");
            return result;
        }

        public override async Task DeleteMessageAsync(int senderId, int messageId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"[DELETE] {messageId}");

            await base.DeleteMessageAsync(senderId, messageId, cancellationToken);
        }
    }
}
