namespace ChatR.DTOs.Friends
{
    public class FriendSidebarItemDto
    {
        public int UserId { get; set; }
        public int? ConversationId { get; set; }
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
