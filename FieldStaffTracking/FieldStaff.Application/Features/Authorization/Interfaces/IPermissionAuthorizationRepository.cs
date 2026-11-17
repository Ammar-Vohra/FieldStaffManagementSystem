using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Authorization.Interfaces
{
    public interface IPermissionAuthorizationRepository
    {
        Task<User?> GetActiveUserAsync(int userId);

        Task<Permission?> GetActivePermissionByCodeAsync(
            string permissionCode);

        Task<UserPermission?> GetValidUserOverrideAsync(
            int userId,
            int permissionId);

        Task<List<string>> GetRolePermissionCodesAsync(
            int roleId);

        Task<List<UserPermission>> GetActiveUserOverridesAsync(
            int userId);

        Task<List<string>> GetAllActivePermissionCodesAsync();
    }
}
