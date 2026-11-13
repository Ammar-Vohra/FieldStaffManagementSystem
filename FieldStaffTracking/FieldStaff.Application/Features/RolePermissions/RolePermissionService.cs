using FieldStaff.Application.Features.Permissions.Interfaces;
using FieldStaff.Application.Features.RolePermissions.DTOs;
using FieldStaff.Application.Features.RolePermissions.Interfaces;
using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RolePermissions
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RolePermissionService(
            IRolePermissionRepository rolePermissionRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<RolePermissionsResponseDto> AssignPermissionsToRoleAsync(
            int roleId,
            AssignRolePermissionsRequestDto request,
            int? addedByUserId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null || role.IsDeleted)
                throw new KeyNotFoundException("Role not found.");

            if (!role.IsActive)
                throw new InvalidOperationException("Cannot assign permissions to an inactive role.");

            if (request.PermissionIds == null || !request.PermissionIds.Any())
                throw new InvalidOperationException("At least one permission must be selected.");

            if (request.PermissionIds.Any(id => id <= 0))
                throw new InvalidOperationException("Invalid permission ID.");

            if (request.PermissionIds.Count != request.PermissionIds.Distinct().Count())
                throw new InvalidOperationException("Duplicate permission IDs are not allowed.");

            var existingAssignments =
                await _rolePermissionRepository.GetExistingAssignmentsAsync(
                    roleId,
                    request.PermissionIds);

            var newAssignments = new List<RolePermission>();

            foreach (var permissionId in request.PermissionIds)
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId);

                if (permission == null)
                    throw new KeyNotFoundException(
                        $"Permission with ID {permissionId} was not found.");

                if (!permission.IsActive)
                    throw new InvalidOperationException(
                        $"Permission '{permission.PermissionName}' is inactive.");

                var existingAssignment = existingAssignments
                    .FirstOrDefault(rp => rp.PermissionId == permissionId);

                if (existingAssignment != null)
                {
                    if (!existingAssignment.IsDeleted && existingAssignment.IsActive)
                        continue;

                    existingAssignment.IsDeleted = false;
                    existingAssignment.IsActive = true;
                    existingAssignment.UpdatedBy = addedByUserId;
                    existingAssignment.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    newAssignments.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = addedByUserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (newAssignments.Any())
                await _rolePermissionRepository.AddRangeAsync(newAssignments);

            await _rolePermissionRepository.SaveChangesAsync();

            return (await GetPermissionsByRoleIdAsync(roleId))!;
        }

        public async Task<RolePermissionsResponseDto?> GetPermissionsByRoleIdAsync(
            int roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null || role.IsDeleted)
                return null;

            var assignments =
                await _rolePermissionRepository.GetByRoleIdAsync(roleId);

            return new RolePermissionsResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Permissions = assignments.Select(rp => new RolePermissionResponseDto
                {
                    RolePermissionId = rp.RolePermissionId,
                    RoleId = rp.RoleId,
                    PermissionId = rp.PermissionId,
                    PermissionName = rp.Permission.PermissionName,
                    PermissionCode = rp.Permission.PermissionCode,
                    IsActive = rp.IsActive
                }).ToList()
            };
        }

        public async Task<bool> RemovePermissionFromRoleAsync(
            int roleId,
            int permissionId,
            int? deletedByUserId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null || role.IsDeleted)
                return false;

            var assignments =
                await _rolePermissionRepository.GetExistingAssignmentsAsync(
                    roleId,
                    new List<int> { permissionId });

            var assignment = assignments.FirstOrDefault(
                rp => !rp.IsDeleted && rp.IsActive);

            if (assignment == null)
                return false;

            assignment.IsDeleted = true;
            assignment.UpdatedBy = deletedByUserId;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _rolePermissionRepository.SaveChangesAsync();

            return true;
        }
    }
}
