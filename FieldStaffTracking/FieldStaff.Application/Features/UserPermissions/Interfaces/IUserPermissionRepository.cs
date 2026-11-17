using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.UserPermissions.Interfaces
{
    public interface IUserPermissionRepository
    {
        Task<UserPermission> CreateAsync(UserPermission userPermission);

        Task<UserPermission?> GetByIdAsync(int userPermissionId);

        Task<List<UserPermission>> GetAllAsync();

        Task<UserPermission> UpdateAsync(UserPermission userPermission);

        Task<bool> ExistsAsync(int userId, int permissionId, int? excludedUserPermissionId = null);

        Task<bool> UserExistsAsync(int userId);

        Task<List<UserPermission>> GetByUserIdAsync(int userId);

        Task<List<UserPermission>> GetByUserIdIncludingDeletedAsync(int userId);


        Task<bool> PermissionExistsAsync(int permissionId);
        Task SaveChangesAsync();

    }
}
