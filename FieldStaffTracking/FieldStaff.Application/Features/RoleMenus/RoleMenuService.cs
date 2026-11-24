
using FieldStaff.Application.Features.Menus.Interfaces;
using FieldStaff.Application.Features.RoleMenus.DTOs;
using FieldStaff.Application.Features.RoleMenus.Interfaces;
using FieldStaff.Application.Features.Roles.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.RoleMenus
{
    public class RoleMenuService : IRoleMenuService
    {
        private readonly IRoleMenuRepository _roleMenuRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMenuRepository _menuRepository;

        public RoleMenuService(
            IRoleMenuRepository roleMenuRepository,
            IRoleRepository roleRepository,
            IMenuRepository menuRepository)
        {
            _roleMenuRepository = roleMenuRepository;
            _roleRepository = roleRepository;
            _menuRepository = menuRepository;
        }

        public async Task<RoleMenuResponseDto> CreateRoleMenuAsync(
            CreateRoleMenuRequestDto request,
            int? addedByUserId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(request.RoleId);

            if (role == null || !role.IsActive)
                throw new InvalidOperationException(
                    "Role not found or inactive.");

            var menu = await _menuRepository.GetByIdAsync(request.MenuId);

            if (menu == null || !menu.IsActive)
                throw new InvalidOperationException(
                    "Menu not found or inactive.");

            bool exists = await _roleMenuRepository.ExistsAsync(
                request.RoleId,
                request.MenuId);

            if (exists)
                throw new InvalidOperationException(
                    "This menu is already assigned to this role.");

            var roleMenu = new RoleMenu
            {
                RoleId = request.RoleId,
                MenuId = request.MenuId,
                
                IsActive = true,
                IsDeleted = false,
                CreatedBy = addedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _roleMenuRepository.CreateAsync(roleMenu);
            await _roleMenuRepository.SaveChangesAsync();

            return MapToResponse(roleMenu, role.RoleName, menu.MenuName);
        }

        public async Task<RoleMenuResponseDto?> GetRoleMenuByIdAsync(
            int roleMenuId)
        {
            var roleMenu = await _roleMenuRepository.GetByIdAsync(roleMenuId);

            if (roleMenu == null)
                return null;

            return MapToResponse(
                roleMenu,
                roleMenu.Role.RoleName,
                roleMenu.Menu.MenuName);
        }

        public async Task<List<RoleMenuResponseDto>> GetAllRoleMenusAsync()
        {
            var roleMenus = await _roleMenuRepository.GetAllAsync();

            return roleMenus
                .Select(rm => MapToResponse(
                    rm,
                    rm.Role.RoleName,
                    rm.Menu.MenuName))
                .ToList();
        }

      
        public async Task<List<RoleMenuResponseDto>> GetMenusByRoleIdAsync(
            int roleId)
        {
            var roleMenus = await _roleMenuRepository.GetByRoleIdAsync(roleId);

           
            var menus = roleMenus.Select(rm => new RoleMenuResponseDto
            {
                RoleMenuId = rm.RoleMenuId,
                RoleId = rm.RoleId,
                RoleName = rm.Role.RoleName,
                MenuId = rm.MenuId,
                Icon = rm.Menu.Icon,
                MenuName = rm.Menu.MenuName,
                MenuPath = rm.Menu.MenuPath,
                PermissionId = rm.Menu.PermissionId,
                PermissionCode = rm.Menu.Permission?.PermissionCode,   
                IsActive = rm.IsActive
            }).ToList();

            var menuLookup = menus.ToDictionary(m => m.MenuId);

            var rootMenus = new List<RoleMenuResponseDto>();

           
            foreach (var menu in menus)
            {
                
                if (roleMenus
                    .First(rm => rm.MenuId == menu.MenuId)
                    .Menu.ParentMenuId is int parentMenuId &&
                    menuLookup.TryGetValue(parentMenuId, out var parent))
                {
                    parent.Children.Add(menu);
                }
                else
                {
                    rootMenus.Add(menu);
                }
            }

            
            SortMenus(rootMenus, roleMenus);

            return rootMenus;
        }

        private static void SortMenus(
            List<RoleMenuResponseDto> menus,
            List<RoleMenu> roleMenus)
        {
            menus.Sort((a, b) =>
            {
                var aOrder = roleMenus.First(rm => rm.MenuId == a.MenuId)
                    .Menu.SortOrder;

                var bOrder = roleMenus.First(rm => rm.MenuId == b.MenuId)
                    .Menu.SortOrder;

                return aOrder.CompareTo(bOrder);
            });

            foreach (var menu in menus)
            {
                SortMenus(menu.Children, roleMenus);
            }
        }

        public async Task<RoleMenuResponseDto?> UpdateRoleMenuAsync(
            int roleMenuId,
            UpdateRoleMenuRequestDto request,
            int? updatedByUserId)
        {
            var roleMenu = await _roleMenuRepository.GetByIdAsync(roleMenuId);

            if (roleMenu == null)
                return null;

            var role = await _roleRepository.GetRoleByIdAsync(request.RoleId);

            if (role == null || !role.IsActive)
                throw new InvalidOperationException(
                    "Role not found or inactive.");

            var menu = await _menuRepository.GetByIdAsync(request.MenuId);

            if (menu == null || !menu.IsActive)
                throw new InvalidOperationException(
                    "Menu not found or inactive.");

            bool exists = await _roleMenuRepository.ExistsAsync(
                request.RoleId,
                request.MenuId,
                roleMenuId);

            if (exists)
                throw new InvalidOperationException(
                    "This menu is already assigned to this role.");

            roleMenu.RoleId = request.RoleId;
            roleMenu.MenuId = request.MenuId;
            roleMenu.IsActive = request.IsActive;
            roleMenu.UpdatedBy = updatedByUserId;
            roleMenu.UpdatedAt = DateTime.UtcNow;

            await _roleMenuRepository.UpdateAsync(roleMenu);
            await _roleMenuRepository.SaveChangesAsync();

            return MapToResponse(
                roleMenu,
                role.RoleName,
                menu.MenuName);
        }

        public async Task<RoleMenuResponseDto?> DeleteRoleMenuAsync(
            int roleMenuId,
            int? deletedByUserId)
        {
            var roleMenu = await _roleMenuRepository.GetByIdAsync(roleMenuId);

            if (roleMenu == null)
                return null;

            roleMenu.IsDeleted = true;
            roleMenu.IsActive = false;
            roleMenu.UpdatedBy = deletedByUserId;
            roleMenu.UpdatedAt = DateTime.UtcNow;

            await _roleMenuRepository.UpdateAsync(roleMenu);
            await _roleMenuRepository.SaveChangesAsync();

            return MapToResponse(
                roleMenu,
                roleMenu.Role.RoleName,
                roleMenu.Menu.MenuName);
        }

        private static RoleMenuResponseDto MapToResponse(
            RoleMenu roleMenu,
            string roleName,
            string menuName)
        {
            return new RoleMenuResponseDto
            {
                RoleMenuId = roleMenu.RoleMenuId,
                RoleId = roleMenu.RoleId,
                RoleName = roleName,
                MenuId = roleMenu.MenuId,
                MenuName = menuName,
                Icon = roleMenu.Menu?.Icon,
                MenuPath = roleMenu.Menu?.MenuPath,
                PermissionId = roleMenu.Menu?.PermissionId,
                PermissionCode = roleMenu.Menu?.Permission?.PermissionCode,
                IsActive = roleMenu.IsActive
            };
        }
    }
}
