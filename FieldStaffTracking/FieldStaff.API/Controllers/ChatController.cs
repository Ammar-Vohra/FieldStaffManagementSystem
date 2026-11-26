using FieldStaff.Application.Features.Chat.DTOs;
using FieldStaff.Application.Features.Chat.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace FieldStaff.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatservice;

        public ChatController(IChatService chatService)
        {
            _chatservice = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request)
        {

            var roleIdClaim = User.FindFirst("RoleId") ?? User.FindFirst("roleId");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId") ?? User.FindFirst("roleId");


            if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out var roleId))
            {
                return Unauthorized(new
                {
                    message = "Role ID not found in authentication token."
                });
            }

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new
                {
                    message = "User ID not found in authentication token."
                });
            }

            var result = await _chatservice.SendMessageAsync(request, roleId, userId);
            return Ok(result);
        }
    }
}
