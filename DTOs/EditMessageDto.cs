namespace ChatR.Server.DTOs
{
    public class EditMessageDto
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public int ConversationId { get; set; }
    }
}
