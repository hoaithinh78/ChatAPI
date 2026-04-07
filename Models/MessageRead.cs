namespace ChatR.Server.Models
{
    public class MessageRead
    {
        public int Id { get; set; }
        public DateTime? ReadAt { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
    }
}
