namespace ChatR.DTOs.Server
{
    public class MyServerDto
    {
        public int ServerId { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string ShortName { get; set; } = string.Empty;
    }
}
