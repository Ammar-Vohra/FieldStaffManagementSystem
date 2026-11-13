
using FieldStaff.Application.Features.Permissions.DTOs;
using FieldStaff.Application.Features.Permissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();

            return Ok(permissions);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);

            if (permission == null)
                return NotFound(new { message = "Permission not found." });

            return Ok(permission);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreatePermission(
            [FromBody] CreatePermissionRequestDto request)
        {
            try
            {
                int? userId = GetCurrentUserId();

                var permission = await _permissionService.CreatePermissionAsync(
                    request,
                    userId);

                return CreatedAtAction(
                    nameof(GetPermissionById),
                    new { id = permission.PermissionId },
                    permission);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(
            int id,
            [FromBody] UpdatePermissionRequestDto request)
        {
            try
            {
                int? userId = GetCurrentUserId();

                var permission = await _permissionService.UpdatePermissionAsync(
                    id,
                    request,
                    userId);

                if (permission == null)
                    return NotFound(new { message = "Permission not found." });

                return Ok(permission);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            int? userId = GetCurrentUserId();

            var permission = await _permissionService.DeletePermissionAsync(
                id,
                userId);

            if (permission == null)
                return NotFound(new { message = "Permission not found." });

            return Ok(permission);
        }

  
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int id))
                return id;

            return null;
        }
    }
}
