
using FieldStaff.Application.Features.Permissions.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly FieldStaffDbContext _context;

        public PermissionRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<Permission> CreateAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
            return permission;
        }

        public async Task<Permission?> GetByIdAsync(int permissionId)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p =>
                    p.PermissionId == permissionId &&
                    !p.IsDeleted);
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.PermissionName)
                .ToListAsync();
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            return permission;
        }

        public async Task<bool> ExistsByCodeAsync(
            string permissionCode,
            int? excludePermissionId = null)
        {
            return await _context.Permissions.AnyAsync(p =>
                p.PermissionCode == permissionCode &&
                !p.IsDeleted &&
                (!excludePermissionId.HasValue ||
                 p.PermissionId != excludePermissionId.Value));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
