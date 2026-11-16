
using FieldStaff.Application.Features.RoleMenus.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Persistence.Repositories
{
    public class RoleMenuRepository : IRoleMenuRepository
    {
        private readonly FieldStaffDbContext _context;

        public RoleMenuRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<RoleMenu> CreateAsync(RoleMenu roleMenu)
        {
            await _context.RoleMenus.AddAsync(roleMenu);
            return roleMenu;
        }

        public async Task<RoleMenu?> GetByIdAsync(int roleMenuId)
        {
            return await _context.RoleMenus
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .FirstOrDefaultAsync(rm => rm.RoleMenuId == roleMenuId);
        }

        public async Task<List<RoleMenu>> GetAllAsync()
        {
            return await _context.RoleMenus
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .OrderBy(rm => rm.RoleId)
                .ThenBy(rm => rm.Menu.SortOrder)
                .ToListAsync();
        }

        public async Task<List<RoleMenu>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RoleMenus
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .Where(rm =>
                    rm.RoleId == roleId &&
                    rm.IsActive &&
                    !rm.IsDeleted &&
                    rm.Menu.IsActive &&
                    !rm.Menu.IsDeleted)
                .OrderBy(rm => rm.Menu.SortOrder)
                .ToListAsync();
        }

        public async Task<RoleMenu> UpdateAsync(RoleMenu roleMenu)
        {
            _context.RoleMenus.Update(roleMenu);
            return roleMenu;
        }

        public async Task<bool> ExistsAsync(
            int roleId,
            int menuId,
            int? excludeRoleMenuId = null)
        {
            return await _context.RoleMenus.AnyAsync(rm =>
                rm.RoleId == roleId &&
                rm.MenuId == menuId &&
                (!excludeRoleMenuId.HasValue ||
                 rm.RoleMenuId != excludeRoleMenuId.Value));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
