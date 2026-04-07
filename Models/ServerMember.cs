namespace ChatR.Server.Models
{
    public class ServerMember
    {
        public int Id { get; set; }
        public DateTime JoinedAt { get; set; }
        public int IsOnline { get; set; }
        public string? Nickname { get; set; }
        public int ServerId { get; set; }
        public int UserId { get; set; }
    }
}
