
using FieldStaff.Application.Features.Permissions.DTOs;

namespace FieldStaff.Application.Features.Permissions.Interfaces
{
    public interface IPermissionService
    {
        Task<PermissionResponseDto> CreatePermissionAsync(
            CreatePermissionRequestDto request,
            int? addedByUserId);

        Task<PermissionResponseDto?> GetPermissionByIdAsync(int permissionId);

        Task<List<PermissionResponseDto>> GetAllPermissionsAsync();

        Task<PermissionResponseDto?> UpdatePermissionAsync(
            int permissionId,
            UpdatePermissionRequestDto request,
            int? updatedByUserId);

        Task<PermissionResponseDto?> DeletePermissionAsync(
            int permissionId,
            int? deletedByUserId);
    }
}
