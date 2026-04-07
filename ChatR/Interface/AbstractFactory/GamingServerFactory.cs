using ChatR.Models;

namespace ChatR.Interface.AbstractFactory
{
    public class GamingServerFactory : IServerSetupFactory
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
                    ChannelName = "party-room",
                    Type = 2,
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Channel
                {
                    ChannelName = "clips",
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
                    RoleName = "Admin",
                    Position = 1,
                    Color = "#FF0000",
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Role
                {
                    RoleName = "Moderator",
                    Position = 2,
                    Color = "#00FF00",
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Role
                {
                    RoleName = "Player",
                    Position = 3,
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
                    Code = "JOIN_VOICE",
                    Description = "Tham gia voice",
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                },
                new Permission
                {
                    Code = "MANAGE_SERVER",
                    Description = "Quản lý server",
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                }
            };
        }

        public List<RolePermission> CreateRolePermissions(List<Role> roles, List<Permission> permissions)
        {
            var admin = roles.First(r => r.RoleName == "Admin");
            var moderator = roles.First(r => r.RoleName == "Moderator");
            var player = roles.First(r => r.RoleName == "Player");

            var sendMessage = permissions.First(p => p.Code == "SEND_MESSAGE");
            var joinVoice = permissions.First(p => p.Code == "JOIN_VOICE");
            var manageServer = permissions.First(p => p.Code == "MANAGE_SERVER");

            return new List<RolePermission>
            {
                new RolePermission { RoleId = admin.RoleId, PermissionId = sendMessage.PermissionId },
                new RolePermission { RoleId = admin.RoleId, PermissionId = joinVoice.PermissionId },
                new RolePermission { RoleId = admin.RoleId, PermissionId = manageServer.PermissionId },

                new RolePermission { RoleId = moderator.RoleId, PermissionId = sendMessage.PermissionId },
                new RolePermission { RoleId = moderator.RoleId, PermissionId = joinVoice.PermissionId },

                new RolePermission { RoleId = player.RoleId, PermissionId = sendMessage.PermissionId },
                new RolePermission { RoleId = player.RoleId, PermissionId = joinVoice.PermissionId }
            };
        }

        public UserRole CreateOwnerUserRole(int ownerId, int serverId, List<Role> roles)
        {
            var ownerRole = roles.First(r => r.RoleName == "Admin");

            return new UserRole
            {
                UserId = ownerId,
                ServerId = serverId,
                RoleId = ownerRole.RoleId
            };
        }
    }
}
