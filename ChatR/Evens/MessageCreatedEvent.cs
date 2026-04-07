using ChatR.Interface;

namespace ChatR.Evens
{
    public class MessageCreatedEvent : IDomainEvent
    {
        public long MessageId { get; }
        public long SenderId { get; }
        public long? ChannelId { get; }
        public long? ConversationId { get; }
        public string Content { get; }
        public DateTime OccurredOn { get; }

        public MessageCreatedEvent(
            long messageId,
            long senderId,
            long? channelId,
            long? conversationId,
            string content)
        {
            MessageId = messageId;
            SenderId = senderId;
            ChannelId = channelId;
            ConversationId = conversationId;
            Content = content;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
