using ChatR.Models;

namespace ChatR.Interface.AbstractFactory
{
    public interface IServerSetupFactory
    {
        List<Channel> CreateDefaultChannels(int serverId);
        List<Role> CreateDefaultRoles(int serverId);
        List<Permission> CreateDefaultPermissions();
        List<RolePermission> CreateRolePermissions(List<Role> roles, List<Permission> permissions);
        UserRole CreateOwnerUserRole(int ownerId, int serverId, List<Role> roles);
    }
}
