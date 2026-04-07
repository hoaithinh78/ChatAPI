namespace ChatR.Server.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? Username { get; set; }

        public string Password { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
        public string? NumberPhone { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
    }
}
