using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.MenuPermissions.Interfaces
{
    public interface IMenuPermissionRepository
    {
        Task<List<MenuPermission>> GetByMenuIdAsync(int menuId);

        Task<List<MenuPermission>> GetExistingAssignmentsAsync(
            int menuId,
            List<int> permissionIds);

        Task AddRangeAsync(List<MenuPermission> menuPermissions);

        Task SaveChangesAsync();
    }
}
