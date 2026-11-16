using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RoleMenus.Interfaces
{
    public interface IRoleMenuRepository
    {
        Task<RoleMenu> CreateAsync(RoleMenu roleMenu);

        Task<RoleMenu?> GetByIdAsync(int roleMenuId);

        Task<List<RoleMenu>> GetAllAsync();

        Task<List<RoleMenu>> GetByRoleIdAsync(int roleId);

        Task<RoleMenu> UpdateAsync(RoleMenu roleMenu);

        Task<bool> ExistsAsync(int roleId, int menuId, int? excludeRoleMenuId = null);
        Task SaveChangesAsync();
    }
}
