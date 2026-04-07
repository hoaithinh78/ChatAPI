namespace ChatR.Server.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int Type { get; set; }
        public string? Content { get; set; }
        public int IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    }
}
