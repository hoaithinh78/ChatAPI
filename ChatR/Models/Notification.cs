namespace ChatR.Models
{
    public class Notification
    {
        public long NotificationId { get; set; }
        public int Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public int IsRead { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public long UserId { get; set; }

        public User? User { get; set; }
    }
}
