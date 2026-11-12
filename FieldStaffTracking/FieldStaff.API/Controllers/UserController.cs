
using FieldStaff.Application.Features.Users.DTOs;
using FieldStaff.Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUserRequestDto request)
        {
            try
            {
                int? createdByUserId = GetCurrentUserId();

                var result = await _userService.CreateUserAsync(
                    request,
                    createdByUserId);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = result.UserId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();

            return Ok(result);
        }


        // GET USERS WITH SEARCH, FILTER, SORT AND PAGINATION
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] UserQueryParameters parameters)
        {
            try
            {
                var result = await _userService.GetPagedUsersAsync(parameters);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(
            int id,
            [FromBody] UpdateUserRequestDto request)
        {
            try
            {
                int? updatedByUserId = GetCurrentUserId();

                var result = await _userService.UpdateUserAsync(
                    id,
                    request,
                    updatedByUserId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "User not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int? deletedByUserId = GetCurrentUserId();

            bool deleted = await _userService.DeleteUserAsync(
                id,
                deletedByUserId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(new
            {
                message = "User deleted successfully."
            });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
