namespace ChatR.Server.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public int ServerId { get; set; }
    }
}
