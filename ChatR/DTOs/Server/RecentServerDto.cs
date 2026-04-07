namespace ChatR.DTOs.Server
{
    public class RecentServerDto
    {
        public int ServerId { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int TotalMembers { get; set; }
        public string LastMessagePreview { get; set; } = string.Empty;
    }
}
