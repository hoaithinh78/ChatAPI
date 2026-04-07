namespace ChatR.DTOs.Attachment
{
    public class UploadAttachmentDto
    {
        public IFormFile File { get; set; } = null!;
        public int ChannelId { get; set; }
        public string? Content { get; set; }
    }
}
