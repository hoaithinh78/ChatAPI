using ChatR.Models;

namespace ChatR.Interface.AbstractFactory
{
    public class StudyServerFactory : IServerSetupFactory
    {
        public List<Channel> CreateDefaultChannels(int serverId)
        {
            return new List<Channel>
            {
                new Channel
                {
                    ChannelName = "general",
                    Type = 1,
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Channel
                {
                    ChannelName = "homework",
                    Type = 1,
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Channel
                {
                    ChannelName = "resources",
                    Type = 1,
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                }
            };
        }

        public List<Role> CreateDefaultRoles(int serverId)
        {
            return new List<Role>
            {
                new Role
                {
                    RoleName = "Teacher",
                    Position = 1,
                    Color = "#FF0000",
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Role
                {
                    RoleName = "Student",
                    Position = 2,
                    Color = "#0000FF",
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                }
            };
        }

        public List<Permission> CreateDefaultPermissions()
        {
            return new List<Permission>
            {
                new Permission
                {
                    Code = "SEND_MESSAGE",
                    Description = "Gửi tin nhắn",
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                },
                new Permission
                {
                    Code = "MANAGE_CHANNEL",
                    Description = "Quản lý kênh",
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                },
                new Permission
                {
                    Code = "MANAGE_ROLE",
                    Description = "Quản lý vai trò",
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                }
            };
        }

        public List<RolePermission> CreateRolePermissions(List<Role> roles, List<Permission> permissions)
        {
            var teacher = roles.First(r => r.RoleName == "Teacher");
            var student = roles.First(r => r.RoleName == "Student");

            var sendMessage = permissions.First(p => p.Code == "SEND_MESSAGE");
            var manageChannel = permissions.First(p => p.Code == "MANAGE_CHANNEL");
            var manageRole = permissions.First(p => p.Code == "MANAGE_ROLE");

            return new List<RolePermission>
            {
                new RolePermission { RoleId = teacher.RoleId, PermissionId = sendMessage.PermissionId },
                new RolePermission { RoleId = teacher.RoleId, PermissionId = manageChannel.PermissionId },
                new RolePermission { RoleId = teacher.RoleId, PermissionId = manageRole.PermissionId },
                new RolePermission { RoleId = student.RoleId, PermissionId = sendMessage.PermissionId }
            };
        }

        public UserRole CreateOwnerUserRole(int ownerId, int serverId, List<Role> roles)
        {
            var ownerRole = roles.OrderBy(r => r.Position).First();

            return new UserRole
            {
                UserId = ownerId,
                ServerId = serverId,
                RoleId = ownerRole.RoleId
            };
        }
    }
}
