namespace ChatR.Server.Models
{
    public class Servers
    {
        public int ServerId { get; set; }
        public string? ServerName { get; set; }
        public int OwnerId { get; set; }
        public string? Description { get; set; }
        public int TotalMembers { get; set; }
        public int OnlineMembers { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? IconUrl { get; set; }
    }
}
