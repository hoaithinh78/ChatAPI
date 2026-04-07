namespace ChatR.DTOs.User
{
    public class CurrentUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public int Status { get; set; }
    }
}
