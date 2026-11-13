using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Application.Features.Menus.DTOs;
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

        public async Task<MenuResponseDto> CreateAsync(CreateMenuRequestDto request, int? createdByUserId)
        {
            await ValidateMenuAsync(request.ParentMenuId, request.MenuPath);

            var menu = new Menu
            {
                ParentMenuId = request.ParentMenuId,
                MenuName = request.MenuName.Trim(),
                MenuPath = NormalizePath(request.MenuPath),
                Icon = request.Icon,
                SortOrder = request.SortOrder,
                IsActive = request.isActive,
                IsDeleted = false,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(menu);
            await _repository.SaveChangesAsync();

            Menu? parentMenu = null;

            if (menu.ParentMenuId.HasValue)
            {
                parentMenu = await _repository.GetByIdAsync(menu.ParentMenuId.Value);
            }

            return MapToResponse(menu, parentMenu);
        }


        public async Task<MenuResponseDto?> GetByIdAsync(int menuId)
        {
            var menu = await _repository.GetByIdAsync(menuId);

            if (menu == null)
                return null;

            return MapToResponse(menu, menu.ParentMenu);
        }

        public async Task<List<MenuResponseDto>> GetAllAsync()
        {
            var menus = await _repository.GetAllAsync();

            return menus.Select(menu =>
                MapToResponse(menu, menu.ParentMenu))
                .ToList();
        }

        public async Task<MenuResponseDto?> UpdateAsync(
            int menuId,
            UpdateMenuRequestDto request,
            int? updatedByUserId)
        {
            var menu = await _repository.GetByIdAsync(menuId);

            if (menu == null)
                return null;

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
            menu.IsActive = request.IsActive;
            menu.UpdatedBy = updatedByUserId;
            menu.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(menu);
            await _repository.SaveChangesAsync();

            Menu? parentMenu = null;

            if (menu.ParentMenuId.HasValue)
            {
                parentMenu = await _repository.GetByIdAsync(menu.ParentMenuId.Value);
            }

            return MapToResponse(menu, parentMenu);
        }

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

        private async Task ValidateMenuAsync(
            int? parentMenuId,
            string? menuPath)
        {
            if (parentMenuId.HasValue)
            {
                var parentMenu = await _repository.GetByIdAsync(parentMenuId.Value);

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

                // Only root-level containers can have children
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

        private static string? NormalizePath(string? menuPath)
        {
            return string.IsNullOrWhiteSpace(menuPath)
                ? null
                : menuPath.Trim();
        }

        private static MenuResponseDto MapToResponse(Menu menu, Menu? parentMenu)
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
                IsAcitve = menu.IsActive
            };
        }

    }
}
