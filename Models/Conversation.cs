namespace ChatR.Server.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public int Type { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
