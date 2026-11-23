
using FieldStaff.API.Authorization.Attributes;
using FieldStaff.Application.Features.UserPermissions.DTOs;
using FieldStaff.Application.Features.Authorization.Interfaces;
using FieldStaff.Application.Features.UserPermissions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionsController : ControllerBase
    {
        private readonly IUserPermissionService _service;
        private readonly IPermissionAuthorizationService _permissionAuthorizationService;

        public UserPermissionsController(IUserPermissionService service, IPermissionAuthorizationService permissionAuthorizationService)
        {
            _permissionAuthorizationService = permissionAuthorizationService;
            _service = service;
        }

        // Get current logged-in User ID from JWT
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

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


        [HttpGet("my-permissions")]
        public async Task<IActionResult> GetMyPermissions()
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            var permissions =
                await _permissionAuthorizationService
                    .GetEffectivePermissionsAsync(userId);

            return Ok(permissions);
        }


        // GET: api/UserPermissions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound(new
                {
                    message = "User permission not found."
                });

            return Ok(result);
        }

        // GET: api/UserPermissions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        // GET: api/UserPermissions/user/2
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var result = await _service.GetByUserIdAsync(userId);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: api/UserPermissions/user/2
        [HttpPut("user/{userId:int}")]
        public async Task<IActionResult> AssignBulk(
            int userId,
            [FromBody] BulkUserPermissionRequestDto request)
        {
            try
            {
                var result = await _service.AssignBulkAsync(
                    userId,
                    request,
                    GetCurrentUserId());

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
                return NotFound(new
                {
                    message = "User permission not found."
                });

            return Ok(new
            {
                message = "User permission deleted successfully."
            });
        }
    }
}
