namespace ChatR.DTOs.Friends
{
    public class FriendListItemDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
    }
}
