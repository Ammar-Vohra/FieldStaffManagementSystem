using FieldStaff.Application.Features.MenuPermissions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.MenuPermissions.Interfaces
{
    public interface IMenuPermissionService
    {
        Task<MenuPermissionsResponseDto> AssignPermissionsToMenuAsync(
            int menuId,
            AssignMenuPermissionsRequestDto request,
            int? addedByUserId);

        Task<MenuPermissionsResponseDto?> GetPermissionsByMenuIdAsync(
            int menuId);

        Task<bool> RemovePermissionFromMenuAsync(
            int menuId,
            int permissionId,
            int? deletedByUserId);
    }
}
