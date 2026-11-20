using FieldStaff.Application.Features.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var roleIdClaim =
                User.FindFirst("RoleId") ??
                User.FindFirst("roleId");

            if (roleIdClaim == null ||
                !int.TryParse(roleIdClaim.Value, out var roleId))
            {
                return Unauthorized(new
                {
                    message = "Role ID not found in authentication token."
                });
            }

            var result = await _dashboardService.GetSummaryForRoleAsync(roleId);

            if (result == null)
            {
                return Forbid();
            }

            return Ok(result);
        }
    }
}