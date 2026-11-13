using FieldStaff.Application.Features.UserPermissions.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Infrastructure.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly FieldStaffDbContext _context;

        public UserPermissionRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<UserPermission> CreateAsync(UserPermission userPermission)
        {
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            return userPermission;
        }

        public async Task<UserPermission?> GetByIdAsync(int userPermissionId)
        {
            return await _context.UserPermissions
                .Include(up => up.User)
                .Include(up => up.Permission)
                .FirstOrDefaultAsync(up =>
                    up.UserPermissionId == userPermissionId &&
                    !up.IsDeleted);
        }

        public async Task<List<UserPermission>> GetAllAsync()
        {
            return await _context.UserPermissions
                .Include(up => up.User)
                .Include(up => up.Permission)
                .Where(up => !up.IsDeleted)
                .OrderByDescending(up => up.UserPermissionId)
                .ToListAsync();
        }

        public async Task<UserPermission> UpdateAsync(UserPermission userPermission)
        {
            _context.UserPermissions.Update(userPermission);
            await _context.SaveChangesAsync();

            return userPermission;
        }

        public async Task<bool> DeleteAsync(int userPermissionId)
        {
            var userPermission = await _context.UserPermissions
                .FirstOrDefaultAsync(up =>
                    up.UserPermissionId == userPermissionId &&
                    !up.IsDeleted);

            if (userPermission == null)
                return false;

            userPermission.IsDeleted = true;
            userPermission.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int userId, int permissionId)
        {
            return await _context.UserPermissions
                .AnyAsync(up =>
                    up.UserId == userId &&
                    up.PermissionId == permissionId &&
                    !up.IsDeleted);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(u =>
                    u.UserId == userId &&
                    !u.IsDeleted);
        }

        public async Task<bool> PermissionExistsAsync(int permissionId)
        {
            return await _context.Permissions
                .AnyAsync(p =>
                    p.PermissionId == permissionId &&
                    !p.IsDeleted);
        }
    }
}
