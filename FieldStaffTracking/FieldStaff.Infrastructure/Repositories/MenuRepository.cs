using FieldStaff.Application.Features.Menus.DTOs;
using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Application.Features.Users.DTOs;
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

        public async Task<(List<Menu> Menus, int TotalRecords)> GetPagedAsync(MenuQueryParameters parameters)
        {
            IQueryable<Menu> query = _context.Menus
                .AsNoTracking()
                .Include(m => m.ParentMenu)
                .Where(m => !m.IsDeleted);

            // Search
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim();

                query = query.Where(m =>
                    EF.Functions.Like(m.MenuName, $"%{search}%") ||
                    (m.MenuPath != null &&
                     EF.Functions.Like(m.MenuPath, $"%{search}%")) ||
                    (m.ParentMenu != null &&
                     EF.Functions.Like(m.ParentMenu.MenuName, $"%{search}%")));
            }

            // Filter by status
            if (parameters.IsActive.HasValue)
            {
                query = query.Where(m =>
                    m.IsActive == parameters.IsActive.Value);
            }

            if (parameters.ParentMenuId.HasValue)
            {
                query = query.Where(m =>
                    m.ParentMenuId == parameters.ParentMenuId.Value);
            }

            // Total records before pagination
            int totalRecords = await query.CountAsync();

            // Sorting
            bool isDescending =
                parameters.SortDirection.Equals(
                    "desc",
                    StringComparison.OrdinalIgnoreCase);

            var orderedQuery = parameters.SortBy.ToLower() switch
            {
                "menuname" => isDescending
                    ? query.OrderByDescending(m => m.MenuName)
                    : query.OrderBy(m => m.MenuName),

                "menupath" => isDescending
                    ? query.OrderByDescending(m => m.MenuPath)
                    : query.OrderBy(m => m.MenuPath),

                "parentmenuname" => isDescending
                    ? query.OrderByDescending(m => m.ParentMenu!.MenuName)
                    : query.OrderBy(m => m.ParentMenu!.MenuName),

                "sortorder" => isDescending
                    ? query.OrderByDescending(m => m.SortOrder)
                    : query.OrderBy(m => m.SortOrder),

                "isactive" => isDescending
                    ? query.OrderByDescending(m => m.IsActive)
                    : query.OrderBy(m => m.IsActive),

                "createdat" => isDescending
                    ? query.OrderByDescending(m => m.CreatedAt)
                    : query.OrderBy(m => m.CreatedAt),


                _ => query.OrderBy(m => m.MenuName)
            };

            // Consistent ordering
            orderedQuery = orderedQuery.ThenBy(m => m.MenuId);

            // Pagination
            var menus = await orderedQuery
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (menus, totalRecords);
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
