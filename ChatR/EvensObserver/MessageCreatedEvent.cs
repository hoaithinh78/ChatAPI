using ChatR.Interface.Observer;

namespace ChatR.Evens
{
    public class MessageCreatedEvent : IDomainEvent
    {
        public int MessageId { get; }         // long → int
        public int SenderId { get; }          // long → int
        public int? ChannelId { get; }        // long? → int?
        public int? ConversationId { get; }   // long? → int?
        public string Content { get; }
        public DateTime OccurredOn { get; }

        public MessageCreatedEvent(
            int messageId,
            int senderId,
            int? channelId,
            int? conversationId,
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