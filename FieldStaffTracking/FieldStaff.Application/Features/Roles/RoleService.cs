using FieldStaff.Application.Features.Roles.DTOs;
using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleResponseDto> CreateRoleAsync(
            CreateRoleRequestDto request,
            int? addedByUserId)
        {
            bool roleExists = await _roleRepository
                .RoleExistsByNameAsync(request.RoleName);

            if (roleExists)
            {
                throw new InvalidOperationException(
                    "A role with this name already exists.");
            }

            var role = new Role
            {
                RoleName = request.RoleName.Trim(),
                IsActive = true,
                CreatedBy = addedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _roleRepository.AddRoleAsync(role);
            await _roleRepository.SaveChangesAsync();

            return MapToResponse(role);
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null)
                return null;

            return MapToResponse(role);
        }

        public async Task<List<RoleResponseDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRoleAsync();

            return roles.Select(MapToResponse).ToList();
        }

        public async Task<RoleResponseDto?> UpdateRoleAsync(
            int roleId,
            UpdateRoleRequestDto request,
            int? updatedByUserId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null)
                return null;

            bool roleExists = await _roleRepository
                .RoleExistsByNameAsync(request.RoleName);

            if (roleExists)
            {
                throw new InvalidOperationException(
                    "Another role with this name already exists.");
            }

            role.RoleName = request.RoleName.Trim();
            role.IsActive = request.IsActive ?? role.IsActive;

            role.UpdatedBy = updatedByUserId;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.SaveChangesAsync();

            return MapToResponse(role);
        }

        public async Task<bool> DeleteRoleAsync(
            int roleId,
            int? deletedByUserId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null)
                return false;

            role.IsDeleted = true;
            role.UpdatedBy = deletedByUserId;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.SaveChangesAsync();

            return true;
        }

        private static RoleResponseDto MapToResponse(Role role)
        {
            return new RoleResponseDto
            {
                Id = role.RoleId,
                RoleName = role.RoleName,
                IsActive = role.IsActive
            };
        }
    }
}