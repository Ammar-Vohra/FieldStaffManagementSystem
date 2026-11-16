
using FieldStaff.Application.Features.RoleMenus.DTOs;
using FieldStaff.Application.Features.RoleMenus.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleMenusController : ControllerBase
    {
        private readonly IRoleMenuService _roleMenuService;

        public RoleMenusController(IRoleMenuService roleMenuService)
        {
            _roleMenuService = roleMenuService;
        }

       
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userId, out int id) ? id : null;
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateRoleMenu(
            [FromBody] CreateRoleMenuRequestDto request)
        {
            try
            {
                var result = await _roleMenuService.CreateRoleMenuAsync(
                    request,
                    GetCurrentUserId());

                return CreatedAtAction(
                    nameof(GetRoleMenuById),
                    new { roleMenuId = result.RoleMenuId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        [HttpGet("{roleMenuId:int}")]
        public async Task<IActionResult> GetRoleMenuById(int roleMenuId)
        {
            var result = await _roleMenuService
                .GetRoleMenuByIdAsync(roleMenuId);

            if (result == null)
                return NotFound(new { message = "Role menu not found." });

            return Ok(result);
        }

        [HttpGet("my-menus")]
        public async Task<IActionResult> GetMyMenus()
        {
            var roleIdClaim = User.FindFirst("RoleId")
                              ?? User.FindFirst("roleId");

            if (roleIdClaim == null ||
                !int.TryParse(roleIdClaim.Value, out int roleId))
            {
                return Unauthorized(new
                {
                    message = "Role ID not found in authentication token."
                });
            }

            var menus = await _roleMenuService.GetMenusByRoleIdAsync(roleId);

            return Ok(menus);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllRoleMenus()
        {
            var result = await _roleMenuService.GetAllRoleMenusAsync();

            return Ok(result);
        }

      
        [HttpPut("{roleMenuId:int}")]
        public async Task<IActionResult> UpdateRoleMenu(
            int roleMenuId,
            [FromBody] UpdateRoleMenuRequestDto request)
        {
            try
            {
                var result = await _roleMenuService.UpdateRoleMenuAsync(
                    roleMenuId,
                    request,
                    GetCurrentUserId());

                if (result == null)
                    return NotFound(new { message = "Role menu not found." });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        [HttpDelete("{roleMenuId:int}")]
        public async Task<IActionResult> DeleteRoleMenu(int roleMenuId)
        {
            var result = await _roleMenuService.DeleteRoleMenuAsync(
                roleMenuId,
                GetCurrentUserId());

            if (result == null)
                return NotFound(new { message = "Role menu not found." });

            return Ok(new
            {
                message = "Role menu deleted successfully.",
                data = result
            });
        }
    }
}
