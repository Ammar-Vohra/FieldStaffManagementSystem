
using FieldStaff.Application.Features.Authorization.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FieldStaff.Application.Features.Authorization.Services
{
    public class PermissionAuthorizationService
        : IPermissionAuthorizationService
    {
        private readonly IPermissionAuthorizationRepository _repository;
        private readonly int _adminRoleId;

        public PermissionAuthorizationService(
            IPermissionAuthorizationRepository repository,
            IConfiguration configuration)
        {
            _repository = repository;

            _adminRoleId = configuration.GetValue<int>(
                "Authorization:AdminRoleId");
        }

        public async Task<bool> HasPermissionAsync(
            int userId,
            string permissionCode)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
                return false;

           
            var user = await _repository.GetActiveUserAsync(userId);

            if (user == null)
                return false;

            
            if (user.RoleId == _adminRoleId)
                return true;

            
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

            
            return (await _repository.GetRolePermissionCodesAsync(
                user.RoleId))
                .Contains(permissionCode,
                    StringComparer.OrdinalIgnoreCase);
        }

        public async Task<List<string>> GetEffectivePermissionsAsync(
            int userId)
        {
          
            var user = await _repository.GetActiveUserAsync(userId);

            if (user == null)
                return new List<string>();

           
            if (user.RoleId == _adminRoleId)
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
