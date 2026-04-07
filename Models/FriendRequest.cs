namespace ChatR.Server.Models
{
    public class FriendRequest
    {
        public int RequestId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
