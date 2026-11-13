using FieldStaff.Application.Features.UserPermissions.DTOs;
using FieldStaff.Application.Features.UserPermissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPermissionsController : ControllerBase
    {
        private readonly IUserPermissionService _service;

        public UserPermissionsController(IUserPermissionService service)
        {
            _service = service;
        }

        // Get current logged-in User ID from JWT
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int id))
                return id;

            return null;
        }

        // POST: api/UserPermissions
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserPermissionRequestDto request)
        {
            try
            {
                var result = await _service.CreateAsync(
                    request,
                    GetCurrentUserId());

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.UserPermissionId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/UserPermissions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "User permission not found." });

            return Ok(result);
        }

        // GET: api/UserPermissions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        // PUT: api/UserPermissions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateUserPermissionRequestDto request)
        {
            try
            {
                var result = await _service.UpdateAsync(
                    id,
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

        // DELETE: api/UserPermissions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(
                id,
                GetCurrentUserId());

            if (!deleted)
                return NotFound(new { message = "User permission not found." });

            return Ok(new
            {
                message = "User permission deleted successfully."
            });
        }
    }
}