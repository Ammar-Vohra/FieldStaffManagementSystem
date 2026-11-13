using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using FieldStaff.Application.Features.MenuPermissions.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Infrastructure.Repositories
{
    public class MenuPermissionRepository : IMenuPermissionRepository
    {
        private readonly FieldStaffDbContext _context;

        public MenuPermissionRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<MenuPermission>> GetByMenuIdAsync(int menuId)
        {
            return await _context.MenuPermissions
                .Include(mp => mp.Permission)
                .Where(mp =>
                    mp.MenuId == menuId &&
                    !mp.IsDeleted &&
                    mp.IsActive &&
                    !mp.Permission.IsDeleted &&
                    mp.Permission.IsActive)
                .OrderBy(mp => mp.Permission.PermissionName)
                .ToListAsync();
        }

       
        public async Task<List<MenuPermission>> GetExistingAssignmentsAsync(
            int menuId,
            List<int> permissionIds)
        {
            return await _context.MenuPermissions
                .Where(mp =>
                    mp.MenuId == menuId &&
                    permissionIds.Contains(mp.PermissionId))
                .ToListAsync();
        }

      
        public async Task AddRangeAsync(List<MenuPermission> menuPermissions)
        {
            await _context.MenuPermissions.AddRangeAsync(menuPermissions);
        }

        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
