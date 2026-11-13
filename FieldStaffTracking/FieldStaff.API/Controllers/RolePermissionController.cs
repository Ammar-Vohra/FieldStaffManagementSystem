using FieldStaff.Application.Features.RolePermissions.DTOs;
using FieldStaff.Application.Features.RolePermissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionController(
            IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out var id) ? id : null;
        }

        [HttpPost("{roleId}/assign")]
        public async Task<IActionResult> AssignPermissions(
            int roleId,
            [FromBody] AssignRolePermissionsRequestDto request)
        {
            try
            {
                var result = await _rolePermissionService.AssignPermissionsToRoleAsync(
                    roleId,
                    request,
                    GetCurrentUserId());

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            var result = await _rolePermissionService
                .GetPermissionsByRoleIdAsync(roleId);

            if (result == null)
                return NotFound(new { message = "Role not found." });

            return Ok(result);
        }

        [HttpDelete("{roleId}/{permissionId}")]
        public async Task<IActionResult> RemovePermission(
            int roleId,
            int permissionId)
        {
            var result = await _rolePermissionService.RemovePermissionFromRoleAsync(
                roleId,
                permissionId,
                GetCurrentUserId());

            if (!result)
                return NotFound(new
                {
                    message = "Role or permission assignment not found."
                });

            return Ok(new
            {
                message = "Permission removed from role successfully."
            });
        }
    }
}