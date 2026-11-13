using FieldStaff.Domain.Entities;
using FieldStaff.Application.Features.RolePermissions.Interfaces;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Infrastructure.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly FieldStaffDbContext _context;

        public RolePermissionRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<List<RolePermission>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp =>
                    rp.RoleId == roleId &&
                    !rp.IsDeleted &&
                    rp.IsActive &&
                    !rp.Permission.IsDeleted &&
                    rp.Permission.IsActive)
                .OrderBy(rp => rp.Permission.PermissionName)
                .ToListAsync();
        }

        public async Task<List<RolePermission>> GetExistingAssignmentsAsync(
            int roleId,
            List<int> permissionIds)
        {
            return await _context.RolePermissions
                .Where(rp =>
                    rp.RoleId == roleId &&
                    permissionIds.Contains(rp.PermissionId))
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<RolePermission> rolePermissions)
        {
            await _context.RolePermissions.AddRangeAsync(rolePermissions);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
