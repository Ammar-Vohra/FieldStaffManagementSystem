using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Roles.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsByNameAsync(string roleName);

        Task<Role?> GetRoleByIdAsync(int roleId);

        Task<List<Role>> GetAllRoleAsync();

        Task AddRoleAsync(Role role);

        Task SaveChangesAsync();
    }
}
