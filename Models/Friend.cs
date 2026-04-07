namespace ChatR.Server.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User UserInfo { get; set; }  // Người dùng (UserId)
        public User FriendInfo { get; set; }  // Bạn bè (FriendId)
    }
}
