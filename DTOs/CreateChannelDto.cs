namespace ChatR.Server.DTOs
{
    public class CreateChannelDto
    {
        public string ChannelName { get; set; }
        public int Type { get; set; }
        public int ServerId { get; set; }
    }
}
