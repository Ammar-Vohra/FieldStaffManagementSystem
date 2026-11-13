
using FieldStaff.Application.Features.MenuPermissions.DTOs;
using FieldStaff.Application.Features.MenuPermissions.Interfaces;
using FieldStaff.Application.Features.MenuPermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers                                                            
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuPermissionsController : ControllerBase
    {
        private readonly IMenuPermissionService _menuPermissionService;

        public MenuPermissionsController(
            IMenuPermissionService menuPermissionService)
        {
            _menuPermissionService = menuPermissionService;
        }

        // POST: api/MenuPermissions/1/assign
        [HttpPost("{menuId}/assign")]
        public async Task<IActionResult> AssignPermissions(
            int menuId,
            [FromBody] AssignMenuPermissionsRequestDto request)
        {
            try
            {
                int? userId = GetCurrentUserId();

                var result = await _menuPermissionService
                    .AssignPermissionsToMenuAsync(
                        menuId,
                        request,
                        userId);

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

        // GET: api/MenuPermissions/1
        [HttpGet("{menuId}")]
        public async Task<IActionResult> GetPermissionsByMenuId(int menuId)
        {
            var result = await _menuPermissionService
                .GetPermissionsByMenuIdAsync(menuId);

            if (result == null)
                return NotFound(new { message = "Menu not found." });

            return Ok(result);
        }

        // DELETE: api/MenuPermissions/1/2
        [HttpDelete("{menuId}/{permissionId}")]
        public async Task<IActionResult> RemovePermission(
            int menuId,
            int permissionId)
        {
            try
            {
                int? userId = GetCurrentUserId();

                bool result = await _menuPermissionService
                    .RemovePermissionFromMenuAsync(
                        menuId,
                        permissionId,
                        userId);

                if (!result)
                    return NotFound(new
                    {
                        message = "Menu permission assignment not found."
                    });

                return Ok(new
                {
                    message = "Permission removed from menu successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET LOGGED-IN USER ID FROM JWT
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int id))
                return id;

            return null;
        }
    }
}
