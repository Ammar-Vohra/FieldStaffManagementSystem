
using FieldStaff.Application.Features.Permissions.DTOs;
using FieldStaff.Application.Features.Permissions.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

       
        public async Task<PermissionResponseDto> CreatePermissionAsync(
            CreatePermissionRequestDto request,
            int? addedByUserId)
        {
            string permissionCode = request.PermissionCode.Trim();

            bool exists = await _permissionRepository
                .ExistsByCodeAsync(permissionCode);

            if (exists)
                throw new InvalidOperationException(
                    "Permission code already exists.");

            var permission = new Permission
            {
                PermissionName = request.PermissionName.Trim(),
                PermissionCode = permissionCode,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = addedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _permissionRepository.CreateAsync(permission);
            await _permissionRepository.SaveChangesAsync();

            return MapToResponse(permission);
        }

     
        public async Task<PermissionResponseDto?> GetPermissionByIdAsync(
            int permissionId)
        {
            var permission = await _permissionRepository
                .GetByIdAsync(permissionId);

            if (permission == null)
                return null;

            return MapToResponse(permission);
        }

        
        public async Task<List<PermissionResponseDto>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();

            return permissions
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<PermissionResponseDto?> UpdatePermissionAsync(
            int permissionId,
            UpdatePermissionRequestDto request,
            int? updatedByUserId)
        {
            var permission = await _permissionRepository
                .GetByIdAsync(permissionId);

            if (permission == null)
                return null;

            string permissionCode = request.PermissionCode.Trim();

            bool exists = await _permissionRepository
                .ExistsByCodeAsync(permissionCode, permissionId);

            if (exists)
                throw new InvalidOperationException(
                    "Permission code already exists.");

            permission.PermissionName = request.PermissionName.Trim();
            permission.PermissionCode = permissionCode;
            permission.IsActive = request.IsActive;
            permission.UpdatedBy = updatedByUserId;
            permission.UpdatedAt = DateTime.UtcNow;

            await _permissionRepository.UpdateAsync(permission);
            await _permissionRepository.SaveChangesAsync();

            return MapToResponse(permission);
        }

        
        public async Task<PermissionResponseDto?> DeletePermissionAsync(
            int permissionId,
            int? deletedByUserId)
        {
            var permission = await _permissionRepository
                .GetByIdAsync(permissionId);

            if (permission == null)
                return null;

            permission.IsDeleted = true;
            permission.UpdatedBy = deletedByUserId;
            permission.UpdatedAt = DateTime.UtcNow;

            await _permissionRepository.UpdateAsync(permission);
            await _permissionRepository.SaveChangesAsync();

            return MapToResponse(permission);
        }

   
        private static PermissionResponseDto MapToResponse(
            Permission permission)
        {
            return new PermissionResponseDto
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                PermissionCode = permission.PermissionCode,
                IsActive = permission.IsActive
            };
        }
    }
}
