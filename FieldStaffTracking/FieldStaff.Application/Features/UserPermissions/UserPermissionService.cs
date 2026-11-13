using FieldStaff.Application.Features.UserPermissions.DTOs;
using FieldStaff.Application.Features.UserPermissions.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.UserPermissions.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionRepository _repository;

        public UserPermissionService(IUserPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserPermissionResponseDto> CreateAsync(
            CreateUserPermissionRequestDto request,
            int? createdByUserId)
        {
            // Validate User
            if (!await _repository.UserExistsAsync(request.UserId))
                throw new InvalidOperationException("User does not exist.");

            // Validate Permission
            if (!await _repository.PermissionExistsAsync(request.PermissionId))
                throw new InvalidOperationException("Permission does not exist.");

            // Check duplicate assignment
            if (await _repository.ExistsAsync(request.UserId, request.PermissionId))
                throw new InvalidOperationException(
                    "This permission is already assigned to the user.");

            var userPermission = new UserPermission
            {
                UserId = request.UserId,
                PermissionId = request.PermissionId,
                IsGranted = request.IsGranted,
                ExpiresAt = request.ExpiresAt,

                IsActive = true,
                IsDeleted = false,

                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(userPermission);

            var result = await _repository.GetByIdAsync(created.UserPermissionId);

            return MapToResponseDto(result!);
        }

        public async Task<UserPermissionResponseDto?> GetByIdAsync(
            int userPermissionId)
        {
            var userPermission = await _repository.GetByIdAsync(userPermissionId);

            if (userPermission == null)
                return null;

            return MapToResponseDto(userPermission);
        }

        public async Task<List<UserPermissionResponseDto>> GetAllAsync()
        {
            var userPermissions = await _repository.GetAllAsync();

            return userPermissions
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<UserPermissionResponseDto> UpdateAsync(
            int userPermissionId,
            UpdateUserPermissionRequestDto request,
            int? updatedByUserId)
        {
            var userPermission = await _repository.GetByIdAsync(userPermissionId);

            if (userPermission == null)
                throw new KeyNotFoundException("User permission not found.");

            // Validate User
            if (!await _repository.UserExistsAsync(request.UserId))
                throw new InvalidOperationException("User does not exist.");

            // Validate Permission
            if (!await _repository.PermissionExistsAsync(request.PermissionId))
                throw new InvalidOperationException("Permission does not exist.");

            // Check duplicate assignment, excluding current record
            var allUserPermissions = await _repository.GetAllAsync();

            bool duplicateExists = allUserPermissions.Any(up =>
                up.UserPermissionId != userPermissionId &&
                up.UserId == request.UserId &&
                up.PermissionId == request.PermissionId);

            if (duplicateExists)
                throw new InvalidOperationException(
                    "This permission is already assigned to the user.");

            userPermission.UserId = request.UserId;
            userPermission.PermissionId = request.PermissionId;
            userPermission.IsGranted = request.IsGranted;
            userPermission.ExpiresAt = request.ExpiresAt;

            userPermission.UpdatedBy = updatedByUserId;
            userPermission.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(userPermission);

            var updated = await _repository.GetByIdAsync(userPermissionId);

            return MapToResponseDto(updated!);
        }

        public async Task<bool> DeleteAsync(
            int userPermissionId,
            int? deletedByUserId)
        {
            var userPermission = await _repository.GetByIdAsync(userPermissionId);

            if (userPermission == null)
                return false;

            userPermission.IsDeleted = true;
            userPermission.IsActive = false;

            userPermission.UpdatedBy = deletedByUserId;
            userPermission.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(userPermission);

            return true;
        }

        // Entity to DTO mapping
        private static UserPermissionResponseDto MapToResponseDto(
            UserPermission userPermission)
        {
            return new UserPermissionResponseDto
            {
                UserPermissionId = userPermission.UserPermissionId,

                UserId = userPermission.UserId,
                UserName = userPermission.User.UserName,

                PermissionId = userPermission.PermissionId,
                PermissionName = userPermission.Permission.PermissionName,
                PermissionCode = userPermission.Permission.PermissionCode,

                IsGranted = userPermission.IsGranted,
                ExpiresAt = userPermission.ExpiresAt,
                IsActive = userPermission.IsActive
            };
        }
    }
}