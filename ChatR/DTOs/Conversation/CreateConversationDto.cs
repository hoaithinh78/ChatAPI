namespace ChatR.DTOs.Conversation
{
    public class CreateConversationDto
    {
        public int Type { get; set; }
        public string? Name { get; set; }
        public List<int>? UserIds { get; set; }
    }
}
