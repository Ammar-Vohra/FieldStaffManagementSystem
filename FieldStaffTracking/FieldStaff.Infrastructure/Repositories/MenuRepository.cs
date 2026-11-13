using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly FieldStaffDbContext _context;

        public MenuRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<Menu?> GetByIdAsync(int menuId)
        {
            return await _context.Menus
                .Include(m => m.ParentMenu)
                .FirstOrDefaultAsync(m => m.MenuId == menuId && !m.IsDeleted);
        }

        public async Task<List<Menu>> GetAllAsync()
        {
            return await _context.Menus
                .AsNoTracking()
                .Include(m => m.ParentMenu)
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.MenuName)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetChildMenusAsync(int parentMenuId)
        {
            return await _context.Menus
                .AsNoTracking()
                .Where(m =>
                    m.ParentMenuId == parentMenuId &&
                    !m.IsDeleted)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<bool> HasChildMenusAsync(int menuId)
        {
            return await _context.Menus.AnyAsync(m =>
                m.ParentMenuId == menuId &&
                !m.IsDeleted);
        }

        public async Task<bool> ExistsAsync(int menuId)
        {
            return await _context.Menus.AnyAsync(m =>
                m.MenuId == menuId &&
                !m.IsDeleted);
        }

        public async Task AddAsync(Menu menu)
        {
            await _context.Menus.AddAsync(menu);
        }

        public Task UpdateAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
