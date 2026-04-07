namespace ChatR.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }   // từ long → int
        public int Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public int IsRead { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }           // từ long → int
        public User? User { get; set; }
    }
}
