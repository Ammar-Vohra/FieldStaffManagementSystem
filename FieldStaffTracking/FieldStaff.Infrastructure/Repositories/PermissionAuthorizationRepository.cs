
using FieldStaff.Application.Features.Authorization.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class PermissionAuthorizationRepository
        : IPermissionAuthorizationRepository
    {
        private readonly FieldStaffDbContext _context;

        public PermissionAuthorizationRepository(
            FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetActiveUserAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.UserId == userId &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.Role.IsActive &&
                    !u.Role.IsDeleted);
        }

        public async Task<Permission?> GetActivePermissionByCodeAsync(
            string permissionCode)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p =>
                    p.PermissionCode == permissionCode &&
                    p.IsActive &&
                    !p.IsDeleted);
        }

        public async Task<UserPermission?> GetValidUserOverrideAsync(
            int userId,
            int permissionId)
        {
            var now = DateTime.UtcNow;

            return await _context.UserPermissions
                .Include(up => up.Permission)
                .FirstOrDefaultAsync(up =>
                    up.UserId == userId &&
                    up.PermissionId == permissionId &&
                    up.IsActive &&
                    !up.IsDeleted &&
                    (up.ExpiresAt == null ||
                     up.ExpiresAt > now) &&
                    up.Permission.IsActive &&
                    !up.Permission.IsDeleted);
        }

        public async Task<List<string>> GetRolePermissionCodesAsync(
            int roleId)
        {
            return await _context.RolePermissions
                .Where(rp =>
                    rp.RoleId == roleId &&
                    rp.IsActive &&
                    !rp.IsDeleted &&
                    rp.Permission.IsActive &&
                    !rp.Permission.IsDeleted)
                .Select(rp => rp.Permission.PermissionCode)
                .ToListAsync();
        }

        public async Task<List<UserPermission>> GetActiveUserOverridesAsync(
            int userId)
        {
            return await _context.UserPermissions
                .Include(up => up.Permission)
                .Where(up =>
                    up.UserId == userId &&
                    up.IsActive &&
                    !up.IsDeleted &&
                    up.Permission.IsActive &&
                    !up.Permission.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllActivePermissionCodesAsync()
        {
            return await _context.Permissions
                .Where(p =>
                    p.IsActive &&
                    !p.IsDeleted)
                .Select(p => p.PermissionCode)
                .ToListAsync();
        }
    }
}
