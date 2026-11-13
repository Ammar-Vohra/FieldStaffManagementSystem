using FieldStaff.Application.Features.RolePermissions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RolePermissions.Interfaces
{
    public interface IRolePermissionService
    {
        Task<RolePermissionsResponseDto> AssignPermissionsToRoleAsync(
    int roleId,
    AssignRolePermissionsRequestDto request,
    int? addedByUserId);

        Task<RolePermissionsResponseDto?> GetPermissionsByRoleIdAsync(int roleId);

        Task<bool> RemovePermissionFromRoleAsync(
            int roleId,
            int permissionId,
            int? deletedByUserId);
    }
}
