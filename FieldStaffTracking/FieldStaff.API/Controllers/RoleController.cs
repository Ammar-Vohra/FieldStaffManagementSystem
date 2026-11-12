using FieldStaff.Application.Features.Roles.DTOs;
using FieldStaff.Application.Features.Roles.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FieldStaff.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        private int? GetCurrentUserIdSafe()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User?.FindFirst("uid")?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("all")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDto request)
        {
            int? currentUserId = GetCurrentUserIdSafe();
            var role = await _roleService.CreateRoleAsync(request, currentUserId);
            return Ok(role);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequestDto request)
        {
            int? currentUserId = GetCurrentUserIdSafe();
            var role = await _roleService.UpdateRoleAsync(id, request, currentUserId);
            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            int? currentUserId = GetCurrentUserIdSafe();
            var result = await _roleService.DeleteRoleAsync(id, currentUserId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
