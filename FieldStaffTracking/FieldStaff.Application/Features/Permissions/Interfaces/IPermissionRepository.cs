
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Permissions.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission> CreateAsync(Permission permission);

        Task<Permission?> GetByIdAsync(int permissionId);

        Task<List<Permission>> GetAllAsync();

        Task<Permission> UpdateAsync(Permission permission);

        Task<bool> ExistsByCodeAsync(string permissionCode, int? excludePermissionId = null);

        Task SaveChangesAsync();
    }
}
