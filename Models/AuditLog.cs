namespace ChatR.Server.Models
{
    public class AuditLog
    {
        public int LogId { get; set; }
        public string? Action { get; set; }
        public string? TargetType { get; set; }
        public int? TargetId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public int? UserId { get; set; }
        public int? ServerId { get; set; }
    }
}
