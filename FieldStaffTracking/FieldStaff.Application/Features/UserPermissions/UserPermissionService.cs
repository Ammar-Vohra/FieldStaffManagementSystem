
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
            if (await _repository.ExistsAsync(
                request.UserId,
                request.PermissionId))
            {
                throw new InvalidOperationException(
                    "This permission is already assigned to the user.");
            }

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

            await _repository.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(
                created.UserPermissionId);

            if (result == null)
                throw new InvalidOperationException(
                    "Failed to retrieve the created user permission.");

            return MapToResponseDto(result);
        }

        public async Task<UserPermissionResponseDto?> GetByIdAsync(
            int userPermissionId)
        {
            var userPermission = await _repository.GetByIdAsync(
                userPermissionId);

            if (userPermission == null)
                return null;

            return MapToResponseDto(userPermission);
        }


        public async Task<List<UserPermissionResponseDto>> GetByUserIdAsync(
            int userId)
        {
            if (!await _repository.UserExistsAsync(userId))
                throw new InvalidOperationException("User does not exist.");

            var permissions = await _repository.GetByUserIdAsync(userId);

            return permissions
                .Where(up => up.IsActive)
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<List<UserPermissionResponseDto>> AssignBulkAsync(
            int userId,
            BulkUserPermissionRequestDto request,
            int? updatedByUserId)
        {
            // Validate user
            if (!await _repository.UserExistsAsync(userId))
                throw new InvalidOperationException("User does not exist.");

            // Prevent duplicate permission IDs
            var permissionIds = request.Permissions
                .Select(p => p.PermissionId)
                .ToList();

            if (permissionIds.Distinct().Count() != permissionIds.Count)
                throw new InvalidOperationException(
                    "Duplicate permission IDs are not allowed.");

            // Validate all permissions before modifying anything
            foreach (var permissionId in permissionIds)
            {
                if (!await _repository.PermissionExistsAsync(permissionId))
                    throw new InvalidOperationException(
                        $"Permission ID {permissionId} does not exist.");
            }

            // Fetch existing assignments, including deleted ones
            var existingPermissions =
                await _repository.GetByUserIdIncludingDeletedAsync(userId);

            var now = DateTime.UtcNow;

            // Soft-delete assignments that are no longer selected
            foreach (var existing in existingPermissions)
            {
                if (!permissionIds.Contains(existing.PermissionId) &&
                    !existing.IsDeleted)
                {
                    existing.IsDeleted = true;
                    existing.IsActive = false;
                    existing.UpdatedBy = updatedByUserId;
                    existing.UpdatedAt = now;

                    await _repository.UpdateAsync(existing);
                }
            }

            // Insert, update or reactivate selected permissions
            foreach (var selected in request.Permissions)
            {
                var existing = existingPermissions
                    .Where(up => up.PermissionId == selected.PermissionId)
                    .OrderByDescending(up => up.UserPermissionId)
                    .FirstOrDefault();

                if (existing == null)
                {
                    var newPermission = new UserPermission
                    {
                        UserId = userId,
                        PermissionId = selected.PermissionId,
                        IsGranted = true,
                        ExpiresAt = selected.ExpiresAt,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = updatedByUserId,
                        CreatedAt = now
                    };

                    await _repository.CreateAsync(newPermission);
                }
                else
                {
                    existing.IsGranted = true;
                    existing.ExpiresAt = selected.ExpiresAt;
                    existing.IsActive = true;
                    existing.IsDeleted = false;
                    existing.UpdatedBy = updatedByUserId;
                    existing.UpdatedAt = now;

                    await _repository.UpdateAsync(existing);
                }
            }

            // One SaveChanges call: EF Core batches these changes
            await _repository.SaveChangesAsync();

            // Return updated assignments
            var updatedPermissions =
                await _repository.GetByUserIdAsync(userId);

            return updatedPermissions
                .Where(up => up.IsActive && !up.IsDeleted)
                .Select(MapToResponseDto)
                .ToList();
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
            var userPermission = await _repository.GetByIdAsync(
                userPermissionId);

            if (userPermission == null)
                throw new KeyNotFoundException(
                    "User permission not found.");

            // Validate User
            if (!await _repository.UserExistsAsync(request.UserId))
                throw new InvalidOperationException("User does not exist.");

            // Validate Permission
            if (!await _repository.PermissionExistsAsync(request.PermissionId))
                throw new InvalidOperationException("Permission does not exist.");

            // Check duplicate assignment, excluding current record
            if (await _repository.ExistsAsync(
                request.UserId,
                request.PermissionId,
                userPermissionId))
            {
                throw new InvalidOperationException(
                    "This permission is already assigned to the user.");
            }

            userPermission.UserId = request.UserId;
            userPermission.PermissionId = request.PermissionId;
            userPermission.IsGranted = request.IsGranted;
            userPermission.ExpiresAt = request.ExpiresAt;

            userPermission.UpdatedBy = updatedByUserId;
            userPermission.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(userPermission);

            // Save changes to database
            await _repository.SaveChangesAsync();

            var updated = await _repository.GetByIdAsync(
                userPermissionId);

            if (updated == null)
                throw new InvalidOperationException(
                    "Failed to retrieve the updated user permission.");

            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteAsync(
            int userPermissionId,
            int? deletedByUserId)
        {
            var userPermission = await _repository.GetByIdAsync(
                userPermissionId);

            if (userPermission == null)
                return false;

            userPermission.IsDeleted = true;
            userPermission.IsActive = false;

            userPermission.UpdatedBy = deletedByUserId;
            userPermission.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(userPermission);

            // Save changes to database
            await _repository.SaveChangesAsync();

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
