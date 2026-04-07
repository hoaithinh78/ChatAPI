namespace ChatR.Server.DTOs
{
    public class SendMessageDto
    {
        public int? ReceiverId { get; set; }       // private chat
        public string? Content { get; set; }       // nếu không có, set ""
        public int? ChannelId { get; set; }        // gửi trong server
        public int? ConversationId { get; set; }   // group chat
    }
}
