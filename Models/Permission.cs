namespace ChatR.Server.Models
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? Status { get; set; }
    }
}
