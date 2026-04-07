using ChatR.Server.Data;
using ChatR.Server.DTOs;
using ChatR.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleServerController : ControllerBase
    {

        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public RoleServerController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet("get-roles/{serverId}")]
        public async Task<IActionResult> GetRoles(int serverId)
        {
            var roles = await _dbContext.Roles
                .Where(r => r.ServerId == serverId)
                .ToListAsync();

            if (roles == null || !roles.Any())
            {
                return NotFound("Không có vai trò nào trong server này.");
            }

            return Ok(roles);
        }
        [HttpGet("get-permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var permissions = await _dbContext.Permissions.ToListAsync();

            if (permissions == null || !permissions.Any())
            {
                return NotFound("Không có quyền nào trong hệ thống.");
            }

            return Ok(permissions);
        }
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var server = await _dbContext.Servers
                .FirstOrDefaultAsync(s => s.ServerId == createRoleDto.ServerId);

            if (server == null)
            {
                return NotFound("Server không tồn tại.");
            }

            var role = new Role
            {
                RoleName = createRoleDto.RoleName,
                Position = createRoleDto.Position,
                Color = createRoleDto.Color,
                ServerId = createRoleDto.ServerId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            return Ok(role);
        }
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
        {
            var userRole = new UserRole
            {
                UserId = assignRoleDto.UserId,
                RoleId = assignRoleDto.RoleId,
                ServerId = assignRoleDto.ServerId
            };

            _dbContext.UserRoles.Add(userRole);
            await _dbContext.SaveChangesAsync();

            return Ok("Vai trò đã được gán cho người dùng.");
        }
        [HttpPost("assign-permission")]
        public async Task<IActionResult> AssignPermissionToRole([FromBody] AssignPermissionDto assignPermissionDto)
        {
            var permission = new RolePermission
            {
                RoleId = assignPermissionDto.RoleId,
                PermissionId = assignPermissionDto.PermissionId
            };

            _dbContext.RolePermissions.Add(permission);
            await _dbContext.SaveChangesAsync();

            return Ok("Quyền đã được gán cho vai trò.");
        }
    }
}
