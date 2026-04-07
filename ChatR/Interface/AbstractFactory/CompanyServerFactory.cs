using ChatR.Models;

namespace ChatR.Interface.AbstractFactory
{
    public class CompanyServerFactory : IServerSetupFactory
    {
        public List<Channel> CreateDefaultChannels(int serverId)
        {
            return new List<Channel>
            {
                new Channel
                {
                    ChannelName = "announcements",
                    Type = 1,
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Channel
                {
                    ChannelName = "general",
                    Type = 1,
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Channel
                {
                    ChannelName = "meeting-room",
                    Type = 2,
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Channel
                {
                    ChannelName = "hr",
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
                    RoleName = "Owner",
                    Position = 1,
                    Color = "#FF0000",
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Role
                {
                    RoleName = "Manager",
                    Position = 2,
                    Color = "#FFA500",
                    CreatedAt = DateTime.UtcNow,
                    ServerId = serverId
                },
                new Role
                {
                    RoleName = "Employee",
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
                },
                new Permission
                {
                    Code = "MANAGE_SERVER",
                    Description = "Quản lý server",
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                },
                new Permission
                {
                    Code = "JOIN_VOICE",
                    Description = "Tham gia voice",
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                }
            };
        }

        public List<RolePermission> CreateRolePermissions(List<Role> roles, List<Permission> permissions)
        {
            var owner = roles.First(r => r.RoleName == "Owner");
            var manager = roles.First(r => r.RoleName == "Manager");
            var employee = roles.First(r => r.RoleName == "Employee");

            var sendMessage = permissions.First(p => p.Code == "SEND_MESSAGE");
            var manageChannel = permissions.First(p => p.Code == "MANAGE_CHANNEL");
            var manageRole = permissions.First(p => p.Code == "MANAGE_ROLE");
            var manageServer = permissions.First(p => p.Code == "MANAGE_SERVER");
            var joinVoice = permissions.First(p => p.Code == "JOIN_VOICE");

            return new List<RolePermission>
            {
                // Owner
                new RolePermission { RoleId = owner.RoleId, PermissionId = sendMessage.PermissionId },
                new RolePermission { RoleId = owner.RoleId, PermissionId = manageChannel.PermissionId },
                new RolePermission { RoleId = owner.RoleId, PermissionId = manageRole.PermissionId },
                new RolePermission { RoleId = owner.RoleId, PermissionId = manageServer.PermissionId },
                new RolePermission { RoleId = owner.RoleId, PermissionId = joinVoice.PermissionId },

                // Manager
                new RolePermission { RoleId = manager.RoleId, PermissionId = sendMessage.PermissionId },
                new RolePermission { RoleId = manager.RoleId, PermissionId = manageChannel.PermissionId },
                new RolePermission { RoleId = manager.RoleId, PermissionId = joinVoice.PermissionId },

                // Employee
                new RolePermission { RoleId = employee.RoleId, PermissionId = sendMessage.PermissionId },
                new RolePermission { RoleId = employee.RoleId, PermissionId = joinVoice.PermissionId }
            };
        }

        public UserRole CreateOwnerUserRole(int ownerId, int serverId, List<Role> roles)
        {
            var ownerRole = roles.First(r => r.RoleName == "Owner");

            return new UserRole
            {
                UserId = ownerId,
                ServerId = serverId,
                RoleId = ownerRole.RoleId
            };
        }
    }
}
