namespace ChatR.Server.Models
{
    public class Channel
    {
        public int ChannelId { get; set; }
        public string? ChannelName { get; set; }
        public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ServerId { get; set; }
    }
}
