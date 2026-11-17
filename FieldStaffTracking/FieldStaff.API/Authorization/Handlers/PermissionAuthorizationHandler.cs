using FieldStaff.API.Authorization.Requirements;
using FieldStaff.Application.Features.Authorization.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FieldStaff.API.Authorization.Handlers
{
    public class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionAuthorizationService _permissionService;

        public PermissionAuthorizationHandler(
            IPermissionAuthorizationService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
                return;

            var userIdClaim = context.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return;

            var hasPermission = await _permissionService.HasPermissionAsync(
                userId,
                requirement.PermissionCode);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}