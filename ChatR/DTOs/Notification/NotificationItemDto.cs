namespace ChatR.DTOs.Notification
{
    public class NotificationItemDto
    {
        public int NotificationId { get; set; }
        public int Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public int IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
