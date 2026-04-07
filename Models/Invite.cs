namespace ChatR.Server.Models
{
    public class Invite
    {
        public int InviteId { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUses { get; set; }
        public int UsedCount { get; set; }
        public int? CreatedBy { get; set; }
        public int? ServerId { get; set; }
    }
}
