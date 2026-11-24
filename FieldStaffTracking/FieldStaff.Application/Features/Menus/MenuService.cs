
using FieldStaff.Application.Features.Menus.DTOs;
using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Menus
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _repository;

        public MenuService(IMenuRepository repository)
        {
            _repository = repository;
        }

        // CREATE MENU
        public async Task<MenuResponseDto> CreateAsync(
            CreateMenuRequestDto request,
            int? createdByUserId)
        {
            await ValidateMenuAsync(request.ParentMenuId, request.MenuPath);
            await ValidatePermissionAsync(request.PermissionId);

            var menu = new Menu
            {
                ParentMenuId = request.ParentMenuId,
                MenuName = request.MenuName.Trim(),
                MenuPath = NormalizePath(request.MenuPath),
                Icon = request.Icon,
                SortOrder = request.SortOrder,
                PermissionId = request.PermissionId,
                IsActive = request.IsActive,
                IsDeleted = false,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(menu);
            await _repository.SaveChangesAsync();

            // Reload menu to retrieve Permission navigation property.
            var savedMenu = await _repository.GetByIdAsync(menu.MenuId);

            return MapToResponse(savedMenu!, savedMenu!.ParentMenu);
        }

        // GET MENU BY ID
        public async Task<MenuResponseDto?> GetByIdAsync(int menuId)
        {
            var menu = await _repository.GetByIdAsync(menuId);

            if (menu == null)
                return null;

            return MapToResponse(menu, menu.ParentMenu);
        }

        // GET ALL MENUS
        public async Task<List<MenuResponseDto>> GetAllAsync()
        {
            var menus = await _repository.GetAllAsync();

            return menus.Select(menu =>
                MapToResponse(menu, menu.ParentMenu))
                .ToList();
        }

        // GET PAGINATED MENUS
        public async Task<PaginatedResponse<MenuResponseDto>> GetPagedMenusAsync(
            MenuQueryParameters parameters)
        {
            if (parameters.PageNumber < 1)
            {
                throw new InvalidOperationException(
                    "Page number must be greater than zero.");
            }

            if (parameters.PageSize < 1 || parameters.PageSize > 100)
            {
                throw new InvalidOperationException(
                    "Page size must be between 1 and 100.");
            }

            var allowedSortColumns = new[]
            {
                "menuname",
                "menupath",
                "parentmenuname",
                "sortorder",
                "isactive",
                "createdat"
            };

            if (!allowedSortColumns.Contains(
                parameters.SortBy,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Invalid sorting column.");
            }

            if (!parameters.SortDirection.Equals(
                "asc",
                StringComparison.OrdinalIgnoreCase) &&
                !parameters.SortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Sort direction must be either 'asc' or 'desc'.");
            }

            parameters.SortBy = parameters.SortBy.ToLowerInvariant();
            parameters.SortDirection = parameters.SortDirection.ToLowerInvariant();

            var (menus, totalRecords) =
                await _repository.GetPagedAsync(parameters);

            int totalPages = (int)Math.Ceiling(
                (double)totalRecords / parameters.PageSize);

            var menuDtos = menus.Select(menu =>
                MapToResponse(menu, menu.ParentMenu))
                .ToList();

            return new PaginatedResponse<MenuResponseDto>
            {
                Data = menuDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasPreviousPage = parameters.PageNumber > 1,
                HasNextPage = parameters.PageNumber < totalPages
            };
        }

        // UPDATE MENU
        public async Task<MenuResponseDto?> UpdateAsync(
            int menuId,
            UpdateMenuRequestDto request,
            int? updatedByUserId)
        {
            var menu = await _repository.GetByIdAsync(menuId);

            if (menu == null)
                return null;

            await ValidatePermissionAsync(request.PermissionId);

            bool hasChildren = await _repository.HasChildMenusAsync(menuId);

            if (hasChildren &&
                (request.ParentMenuId.HasValue ||
                 !string.IsNullOrWhiteSpace(request.MenuPath)))
            {
                throw new InvalidOperationException(
                    "A parent menu with child menus must remain a container without a URL.");
            }

            await ValidateMenuAsync(request.ParentMenuId, request.MenuPath);

            menu.ParentMenuId = request.ParentMenuId;
            menu.MenuName = request.MenuName.Trim();
            menu.MenuPath = NormalizePath(request.MenuPath);
            menu.Icon = request.Icon;
            menu.SortOrder = request.SortOrder;
            menu.PermissionId = request.PermissionId;
            menu.IsActive = request.IsActive;
            menu.UpdatedBy = updatedByUserId;
            menu.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(menu);
            await _repository.SaveChangesAsync();

            // Reload to retrieve updated Permission navigation property.
            var updatedMenu = await _repository.GetByIdAsync(menuId);

            return MapToResponse(updatedMenu!, updatedMenu!.ParentMenu);
        }

        // DELETE MENU
        public async Task<bool> DeleteAsync(
            int menuId,
            int? deletedByUserId)
        {
            var menu = await _repository.GetByIdAsync(menuId);

            if (menu == null)
                return false;

            bool hasChildren = await _repository.HasChildMenusAsync(menuId);

            if (hasChildren)
            {
                throw new InvalidOperationException(
                    "Cannot delete this menu because it has child menus. Delete or reassign the child menus first.");
            }

            menu.IsDeleted = true;
            menu.IsActive = false;
            menu.UpdatedBy = deletedByUserId;
            menu.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(menu);
            await _repository.SaveChangesAsync();

            return true;
        }

        // VALIDATE PERMISSION
        private async Task ValidatePermissionAsync(int? permissionId)
        {
            // Parent/container menus can have no permission.
            if (!permissionId.HasValue)
                return;

            bool permissionExists =
                await _repository.PermissionExistsAsync(permissionId.Value);

            if (!permissionExists)
            {
                throw new InvalidOperationException(
                    "The selected permission does not exist or is inactive.");
            }
        }

        // VALIDATE MENU
        private async Task ValidateMenuAsync(
            int? parentMenuId,
            string? menuPath)
        {
            if (parentMenuId.HasValue)
            {
                var parentMenu = await _repository.GetByIdAsync(
                    parentMenuId.Value);

                if (parentMenu == null)
                {
                    throw new KeyNotFoundException(
                        "Parent menu not found.");
                }

                if (!parentMenu.IsActive)
                {
                    throw new InvalidOperationException(
                        "Cannot assign an inactive menu as a parent.");
                }

                if (parentMenu.ParentMenuId.HasValue ||
                    !string.IsNullOrWhiteSpace(parentMenu.MenuPath))
                {
                    throw new InvalidOperationException(
                        "Only root-level container menus can have child menus.");
                }

                if (string.IsNullOrWhiteSpace(menuPath))
                {
                    throw new InvalidOperationException(
                        "Child menus must have a valid menu path.");
                }
            }
        }

        // NORMALIZE PATH
        private static string? NormalizePath(string? menuPath)
        {
            return string.IsNullOrWhiteSpace(menuPath)
                ? null
                : menuPath.Trim();
        }

        // MAP ENTITY TO DTO
        private static MenuResponseDto MapToResponse(
            Menu menu,
            Menu? parentMenu)
        {
            return new MenuResponseDto
            {
                MenuId = menu.MenuId,
                ParentMenuId = menu.ParentMenuId,
                ParentMenuName = parentMenu?.MenuName,
                MenuName = menu.MenuName,
                MenuPath = menu.MenuPath,
                Icon = menu.Icon,
                SortOrder = menu.SortOrder,
                PermissionId = menu.PermissionId,
                PermissionCode = menu.Permission?.PermissionCode,
                IsActive = menu.IsActive
            };
        }
    }
}
