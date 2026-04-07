namespace ChatR.Server.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public int IsDeleted { get; set; }
        public int? ChannelId { get; set; }
        public int? ConversationId { get; set; }
    }
}
