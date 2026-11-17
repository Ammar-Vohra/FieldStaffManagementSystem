using FieldStaff.API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace FieldStaff.API.Authorization.Providers
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string PolicyPrefix = "Permission:";

        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider =
                new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(
                PolicyPrefix,
                StringComparison.OrdinalIgnoreCase))
            {
                var permissionCode = policyName.Substring(PolicyPrefix.Length);

                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(
                        new PermissionRequirement(permissionCode))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}