using FieldStaff.Application.Features.Roles.DTOs;

namespace FieldStaff.Application.Features.Roles.Interfaces
{
        public interface IRoleService
        {
            Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto request, int? addedByUserId);

            Task<RoleResponseDto?> GetRoleByIdAsync(int roleId);

            Task<List<RoleResponseDto>> GetAllRolesAsync();

            Task<RoleResponseDto?> UpdateRoleAsync(int roleId, UpdateRoleRequestDto request, int? updatedByUserId);

            Task<bool> DeleteRoleAsync(int roleId, int? deletedByUserId);
        }
}