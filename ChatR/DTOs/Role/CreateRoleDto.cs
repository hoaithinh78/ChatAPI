namespace ChatR.DTOs.Role
{
    public class CreateRoleDto
    {
        public string RoleName { get; set; }
        public int Position { get; set; }
        public string Color { get; set; }
        public int ServerId { get; set; }
    }
}
