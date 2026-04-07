namespace ChatR.Server.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int? Position { get; set; }
        public string? Color { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ServerId { get; set; }
    }
}
