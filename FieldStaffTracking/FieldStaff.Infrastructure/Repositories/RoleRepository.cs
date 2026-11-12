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

        public async Task<bool> RoleExistsByNameAsync(string roleName)
        {
            var query = _context.Roles.AsQueryable();

            return await query.AnyAsync(r =>
                r.RoleName.ToLower() == roleName.Trim().ToLower());
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<List<Role>> GetAllRoleAsync()
        {
            return await _context.Roles
                .AsNoTracking()
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
