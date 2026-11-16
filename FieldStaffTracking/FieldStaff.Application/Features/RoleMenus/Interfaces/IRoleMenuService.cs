using FieldStaff.Application.Features.RoleMenus.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RoleMenus.Interfaces
{
    public interface IRoleMenuService
    {
        Task<RoleMenuResponseDto> CreateRoleMenuAsync(
            CreateRoleMenuRequestDto request,
            int? addedByUserId);

        Task<RoleMenuResponseDto?> GetRoleMenuByIdAsync(int roleMenuId);

        Task<List<RoleMenuResponseDto>> GetAllRoleMenusAsync();
        Task<List<RoleMenuResponseDto>> GetMenusByRoleIdAsync(int roleId);

        Task<RoleMenuResponseDto?> UpdateRoleMenuAsync(
            int roleMenuId,
            UpdateRoleMenuRequestDto request,
            int? updatedByUserId);

        Task<RoleMenuResponseDto?> DeleteRoleMenuAsync(
            int roleMenuId,
            int? deletedByUserId);
    }
}
