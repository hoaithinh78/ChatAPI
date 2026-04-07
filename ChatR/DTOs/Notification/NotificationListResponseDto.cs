namespace ChatR.DTOs.Notification
{
    public class NotificationListResponseDto
    {
        public List<NotificationItemDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }
}
