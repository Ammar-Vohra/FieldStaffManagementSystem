using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly FieldStaffDbContext _context;

        public RoleRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RoleExistsByNameAsync(
            string roleName,
            int? excludedRoleId = null)
        {
            return await _context.Roles.AnyAsync(r =>
                r.RoleName.ToLower() == roleName.Trim().ToLower()
                && !r.IsDeleted
                && (!excludedRoleId.HasValue ||
                    r.RoleId != excludedRoleId.Value));
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r =>
                    r.RoleId == roleId && !r.IsDeleted);
        }

        public async Task<List<Role>> GetAllRoleAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.RoleName)
                .ToListAsync();
        }

        public async Task AddRoleAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}