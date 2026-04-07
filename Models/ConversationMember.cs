namespace ChatR.Server.Models
{
    public class ConversationMember
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int UserId { get; set; }

        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
