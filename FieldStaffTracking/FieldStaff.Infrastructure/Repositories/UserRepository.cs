
using FieldStaff.Application.Features.Users.DTOs;
using FieldStaff.Application.Features.Users.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FieldStaffDbContext _context;

        public UserRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.UserId == userId && !u.IsDeleted);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Email == email && !u.IsDeleted);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<(List<User> Users, int TotalRecords)> GetPagedAsync(UserQueryParameters parameters)
        {
            IQueryable<User> query = _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted);

    
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim();

                query = query.Where(u =>
                    EF.Functions.Like(u.UserName, $"%{search}%") ||
                    EF.Functions.Like(u.Email, $"%{search}%"));
            }

            
            if (parameters.RoleId.HasValue)
            {
                query = query.Where(u =>
                    u.RoleId == parameters.RoleId.Value);
            }

            
            if (parameters.IsActive.HasValue)
            {
                query = query.Where(u =>
                    u.IsActive == parameters.IsActive.Value);
            }

            
            int totalRecords = await query.CountAsync();

           
            bool isDescending =
                parameters.SortDirection.Equals(
                    "desc",
                    StringComparison.OrdinalIgnoreCase);

            var orderedQuery = parameters.SortBy.ToLower() switch
            {
                "username" => isDescending
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName),

                "email" => isDescending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),

                "rolename" => isDescending
                    ? query.OrderByDescending(u => u.Role!.RoleName)
                    : query.OrderBy(u => u.Role!.RoleName),

                "isactive" => isDescending
                    ? query.OrderByDescending(u => u.IsActive)
                    : query.OrderBy(u => u.IsActive),

                "createdat" => isDescending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),

                _ => query.OrderBy(u => u.UserName)
            };


            orderedQuery = orderedQuery.ThenBy(u => u.UserId);


            var users = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (users, totalRecords);
        }


        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeUserId = null)
        {
            return await _context.Users.AnyAsync(u =>
                u.Email == email &&
                !u.IsDeleted &&
                (!excludeUserId.HasValue ||
                 u.UserId != excludeUserId.Value));
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return user;
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
