
using FieldStaff.Application.Features.Authorization.Interfaces;

namespace FieldStaff.Application.Features.Authorization.Services
{
    public class PermissionAuthorizationService
        : IPermissionAuthorizationService
    {
        private readonly IPermissionAuthorizationRepository _repository;

        public PermissionAuthorizationService(
            IPermissionAuthorizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> HasPermissionAsync(
            int userId,
            string permissionCode)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
                return false;

            permissionCode = permissionCode.Trim();

            var user = await _repository.GetActiveUserAsync(userId);

            if (user == null)
                return false;

            
            if (string.Equals(
                user.Role?.RoleName,
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var permission =
                await _repository.GetActivePermissionByCodeAsync(
                    permissionCode);

            if (permission == null)
                return false;

            
            var userOverride =
                await _repository.GetValidUserOverrideAsync(
                    userId,
                    permission.PermissionId);

            if (userOverride != null)
                return userOverride.IsGranted;

            
            var rolePermissions =
                await _repository.GetRolePermissionCodesAsync(
                    user.RoleId);

            return rolePermissions.Contains(
                permissionCode,
                StringComparer.OrdinalIgnoreCase);
        }

        public async Task<List<string>> GetEffectivePermissionsAsync(
            int userId)
        {
            var user = await _repository.GetActiveUserAsync(userId);

            if (user == null)
                return new List<string>();

            
            if (string.Equals(
                user.Role?.RoleName,
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                return await _repository
                    .GetAllActivePermissionCodesAsync();
            }

            
            var effectivePermissions = new HashSet<string>(
                await _repository.GetRolePermissionCodesAsync(
                    user.RoleId),
                StringComparer.OrdinalIgnoreCase);

            
            var overrides =
                await _repository.GetActiveUserOverridesAsync(userId);

            var now = DateTime.UtcNow;

            foreach (var userOverride in overrides)
            {
                if (userOverride.ExpiresAt.HasValue &&
                    userOverride.ExpiresAt.Value <= now)
                {
                    continue;
                }

                var permissionCode =
                    userOverride.Permission.PermissionCode;

                if (userOverride.IsGranted)
                {
                    effectivePermissions.Add(permissionCode);
                }
                else
                {
                    effectivePermissions.Remove(permissionCode);
                }
            }

            return effectivePermissions
                .OrderBy(p => p)
                .ToList();
        }
    }
}
