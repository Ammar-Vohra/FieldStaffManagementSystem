
namespace FieldStaff.Application.Features.Authorization.Interfaces
{
    public interface IPermissionAuthorizationService
    {
        Task<bool> HasPermissionAsync(
            int userId,
            string permissionCode);

        Task<List<string>> GetEffectivePermissionsAsync(
            int userId);
    }
}
