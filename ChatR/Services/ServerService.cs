using ChatR.Data;
using ChatR.DTOs;
using ChatR.DTOs.Server;
using ChatR.Interface.AbstractFactory;
using ChatR.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Services
{
    public class ServerService
    {
        private readonly AppDbContext _dbContext;
        private readonly ServerSetupFactoryProvider _factoryProvider;

        public ServerService(AppDbContext dbContext, ServerSetupFactoryProvider factoryProvider)
        {
            _dbContext = dbContext;
            _factoryProvider = factoryProvider;
        }

        public async Task<object> CreateServerAsync(int userId, CreateServerDto createServerDto)
        {
            if (string.IsNullOrWhiteSpace(createServerDto.ServerName))
            {
                throw new Exception("Tên server không được để trống.");
            }

            var server = new Servers
            {
                ServerName = createServerDto.ServerName.Trim(),
                OwnerId = userId,
                Description = createServerDto.Description,
                IconUrl = createServerDto.IconUrl,
                CreatedAt = DateTime.UtcNow,
                TotalMembers = 0, // để trigger Oracle tự cộng
                OnlineMembers = 0,
                Score = 0
            };

            _dbContext.Servers.Add(server);
            await _dbContext.SaveChangesAsync();

            var factory = _factoryProvider.GetFactory(createServerDto.TemplateType);

            var serverMember = new ServerMember
            {
                ServerId = server.ServerId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
                IsOnline = 0,
                Nickname = null
            };

            _dbContext.ServerMembers.Add(serverMember);

            var channels = factory.CreateDefaultChannels(server.ServerId);
            _dbContext.Channels.AddRange(channels);

            var roles = factory.CreateDefaultRoles(server.ServerId);
            _dbContext.Roles.AddRange(roles);

            await _dbContext.SaveChangesAsync();

            var permissionsFromFactory = factory.CreateDefaultPermissions();

            foreach (var permission in permissionsFromFactory)
            {
                var existingPermission = await _dbContext.Permissions
                    .FirstOrDefaultAsync(p => p.Code == permission.Code);

                if (existingPermission == null)
                {
                    _dbContext.Permissions.Add(permission);
                }
            }

            await _dbContext.SaveChangesAsync();

            var dbPermissions = new List<Permission>();

            foreach (var item in permissionsFromFactory)
            {
                var permissionInDb = await _dbContext.Permissions
                    .FirstOrDefaultAsync(p => p.Code == item.Code);

                if (permissionInDb != null)
                {
                    dbPermissions.Add(permissionInDb);
                }
            }

            var rolePermissions = factory.CreateRolePermissions(roles, dbPermissions);
            _dbContext.RolePermissions.AddRange(rolePermissions);

            var ownerUserRole = factory.CreateOwnerUserRole(userId, server.ServerId, roles);
            _dbContext.UserRoles.Add(ownerUserRole);

            await _dbContext.SaveChangesAsync();

            return new
            {
                Server = server,
                DefaultChannels = channels,
                DefaultRoles = roles
            };
        }

        public async Task<List<ServerMember>> GetServerMembersAsync(int serverId)
        {
            var members = await _dbContext.ServerMembers
                .Where(sm => sm.ServerId == serverId)
                .ToListAsync();

            return members;
        }

        public async Task<string> AddMemberAsync(AddMemberDto addMemberDto)
        {
            var server = await _dbContext.Servers
                .FirstOrDefaultAsync(s => s.ServerId == addMemberDto.ServerId);

            if (server == null)
            {
                throw new Exception("Server không tồn tại.");
            }

            var exists = await _dbContext.ServerMembers.AnyAsync(sm =>
                sm.ServerId == addMemberDto.ServerId &&
                sm.UserId == addMemberDto.UserId);

            if (exists)
            {
                throw new Exception("Người dùng đã ở trong server.");
            }

            var member = new ServerMember
            {
                ServerId = addMemberDto.ServerId,
                UserId = addMemberDto.UserId,
                JoinedAt = DateTime.UtcNow,
                IsOnline = 0
            };

            _dbContext.ServerMembers.Add(member);

            // Không cộng server.TotalMembers ở đây nữa
            // vì trigger Oracle trg_add_member sẽ tự cộng

            await _dbContext.SaveChangesAsync();

            return "Thành viên đã được thêm vào server.";
        }
    }
}