using ChatR.Enums;

namespace ChatR.DTOs.Server
{
    public class CreateServerDto
    {
        public string ServerName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ServerTemplateType TemplateType { get; set; } = ServerTemplateType.Study;
    }
}
