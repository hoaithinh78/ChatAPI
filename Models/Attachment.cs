namespace ChatR.Server.Models
{
    public class Attachment
    {
        public int AttachId { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public decimal? FileSize { get; set; }
        public int MessageId { get; set; }
    }
}
