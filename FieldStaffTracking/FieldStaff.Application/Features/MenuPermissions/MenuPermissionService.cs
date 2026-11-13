using FieldStaff.Application.Features.MenuPermissions.DTOs;
using FieldStaff.Application.Features.MenuPermissions.Interfaces;
using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Application.Features.Permissions.Interfaces;
using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.MenuPermissions
{
    public class MenuPermissionService : IMenuPermissionService
    {
        private readonly IMenuPermissionRepository _menuPermissionRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IPermissionRepository _permissionRepository;

        public MenuPermissionService(
            IMenuPermissionRepository menuPermissionRepository,
            IMenuRepository menuRepository,
            IPermissionRepository permissionRepository)
        {
            _menuPermissionRepository = menuPermissionRepository;
            _menuRepository = menuRepository;
            _permissionRepository = permissionRepository;
        }

      
        public async Task<MenuPermissionsResponseDto> AssignPermissionsToMenuAsync(
            int menuId,
            AssignMenuPermissionsRequestDto request,
            int? addedByUserId)
        {
            
            var menu = await _menuRepository.GetByIdAsync(menuId);

            if (menu == null)
                throw new KeyNotFoundException("Menu not found.");

            if (!menu.IsActive)
                throw new InvalidOperationException("Cannot assign permissions to an inactive menu.");

            
            if (request.PermissionIds == null || request.PermissionIds.Count == 0)
                throw new InvalidOperationException("At least one permission is required.");

          
            if (request.PermissionIds.Distinct().Count() != request.PermissionIds.Count)
                throw new InvalidOperationException("Duplicate permission IDs are not allowed.");

            if (request.PermissionIds.Any(id => id <= 0))
                throw new InvalidOperationException("Invalid permission ID.");

         
            foreach (int permissionId in request.PermissionIds)
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId);

                if (permission == null)
                    throw new KeyNotFoundException(
                        $"Permission with ID {permissionId} not found.");

                if (!permission.IsActive)
                    throw new InvalidOperationException(
                        $"Permission with ID {permissionId} is inactive.");
            }

        
            var existingAssignments =
                await _menuPermissionRepository.GetExistingAssignmentsAsync(
                    menuId,
                    request.PermissionIds);

            var newAssignments = new List<MenuPermission>();

            foreach (int permissionId in request.PermissionIds)
            {
                var existingAssignment = existingAssignments
                    .FirstOrDefault(mp => mp.PermissionId == permissionId);

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
                    
                    newAssignments.Add(new MenuPermission
                    {
                        MenuId = menuId,
                        PermissionId = permissionId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = addedByUserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            
            if (newAssignments.Count > 0)
            {
                await _menuPermissionRepository.AddRangeAsync(newAssignments);
            }

            await _menuPermissionRepository.SaveChangesAsync();

            
            return (await GetPermissionsByMenuIdAsync(menuId))!;
        }

        
        public async Task<MenuPermissionsResponseDto?> GetPermissionsByMenuIdAsync(
            int menuId)
        {
            var menu = await _menuRepository.GetByIdAsync(menuId);

            if (menu == null)
                return null;

            var assignments = await _menuPermissionRepository
                .GetByMenuIdAsync(menuId);

            return new MenuPermissionsResponseDto
            {
                MenuId = menu.MenuId,
                MenuName = menu.MenuName,

                Permissions = assignments.Select(mp =>
                    new MenuPermissionResponseDto
                    {
                        MenuPermissionId = mp.MenuPermissionId,
                        MenuId = mp.MenuId,
                        PermissionId = mp.PermissionId,
                        PermissionName = mp.Permission.PermissionName,
                        PermissionCode = mp.Permission.PermissionCode,
                        IsActive = mp.IsActive
                    }).ToList()
            };
        }

        
        public async Task<bool> RemovePermissionFromMenuAsync(
            int menuId,
            int permissionId,
            int? deletedByUserId)
        {
           
            var menu = await _menuRepository.GetByIdAsync(menuId);

            if (menu == null)
                throw new KeyNotFoundException("Menu not found.");

            
            var assignments =
                await _menuPermissionRepository.GetExistingAssignmentsAsync(
                    menuId,
                    new List<int> { permissionId });

            var assignment = assignments.FirstOrDefault();

            if (assignment == null || assignment.IsDeleted)
                return false;

           
            assignment.IsDeleted = true;
            assignment.UpdatedBy = deletedByUserId;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _menuPermissionRepository.SaveChangesAsync();

            return true;
        }
    }
}
