using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Roles.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsByNameAsync(
            string roleName,
            int? excludedRoleId = null);

        Task<Role?> GetRoleByIdAsync(int roleId);

        Task<List<Role>> GetAllRoleAsync();

        Task AddRoleAsync(Role role);

        Task SaveChangesAsync();
    }
}