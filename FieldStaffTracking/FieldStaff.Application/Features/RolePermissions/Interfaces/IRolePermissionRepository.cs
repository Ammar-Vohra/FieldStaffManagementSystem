using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RolePermissions.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<List<RolePermission>> GetByRoleIdAsync(int roleId);

        Task<List<RolePermission>> GetExistingAssignmentsAsync(
            int roleId,
            List<int> permissionIds);

        Task AddRangeAsync(List<RolePermission> rolePermissions);

        Task SaveChangesAsync();
    }
}
