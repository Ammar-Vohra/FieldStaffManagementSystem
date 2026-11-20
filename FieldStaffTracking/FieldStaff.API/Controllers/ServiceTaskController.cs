
using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.ServiceTasks.Interfaces;
using FieldStaff.Application.Features.TaskStatusHistories.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceTaskController : ControllerBase
    {
        private readonly IServiceTaskService _serviceTaskService;

        public ServiceTaskController(IServiceTaskService serviceTaskService)
        {
            _serviceTaskService = serviceTaskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceTask(
            [FromBody] CreateServiceTaskRequestDto request)
        {
            try
            {
                int? createdByUserId = GetCurrentUserId();

                if (!createdByUserId.HasValue)
                {
                    return Unauthorized(new
                    {
                        message = "Unable to identify the logged-in user."
                    });
                }

                var result = await _serviceTaskService.CreateServiceTaskAsync(
                    request,
                    createdByUserId.Value);

                return CreatedAtAction(
                    nameof(GetServiceTaskById),
                    new { id = result.ServiceTaskId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceTasks(
            [FromQuery] ServiceTaskQueryParameters parameters)
        {
            try
            {
                var result = await _serviceTaskService.GetPagedServiceTasksAsync(
                    parameters);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetServiceTaskById(int id)
        {
            var result = await _serviceTaskService.GetServiceTaskByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Service task not found."
                });
            }

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateServiceTask(
            int id,
            [FromBody] UpdateServiceTaskRequestDto request)
        {
            try
            {
                int? updatedByUserId = GetCurrentUserId();

                if (!updatedByUserId.HasValue)
                {
                    return Unauthorized(new
                    {
                        message = "Unable to identify the logged-in user."
                    });
                }

                var result = await _serviceTaskService.UpdateServiceTaskAsync(
                    id,
                    request,
                    updatedByUserId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Service task not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("MyTasks")]
        public async Task<IActionResult> GetMyTasks([FromQuery] ServiceTaskQueryParameters parameters)
        {
            try
            {
                int? staffId = GetCurrentUserId();

                if (!staffId.HasValue)
                {
                    return Unauthorized(new
                    {
                        message = "Unable to identify the logged-in user."
                    });
                }

                var result = await _serviceTaskService.GetMyTasksAsync(
                    staffId.Value,
                    parameters);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("MyTasks/{id:int}")]
        public async Task<IActionResult> GetMyTaskDetails(int id)
        {
            int? staffId = GetCurrentUserId();

            if (!staffId.HasValue)
            {
                return Unauthorized(new
                {
                    message = "Unable to identify the logged-in user."
                });
            }

            var result = await _serviceTaskService.GetMyTaskDetailsAsync(
                id,
                staffId.Value);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Task not found or not assigned to you."
                });
            }

            return Ok(result);
        }

        [HttpPut("MyTasks/{id:int}/Status")]
        public async Task<IActionResult> UpdateMyTaskStatus(
    int id,
    [FromBody] UpdateTaskStatusHistoryRequestDto request)
        {
            try
            {
                int? staffId = GetCurrentUserId();

                if (!staffId.HasValue)
                {
                    return Unauthorized(new
                    {
                        message = "Unable to identify the logged-in user."
                    });
                }

                var result = await _serviceTaskService.UpdateMyTaskStatusAsync(
                    id,
                    request,
                    staffId.Value);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Task not found or not assigned to you."
                    });
                }

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


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteServiceTask(int id)
        {
            int? deletedByUserId = GetCurrentUserId();

            if (!deletedByUserId.HasValue)
            {
                return Unauthorized(new
                {
                    message = "Unable to identify the logged-in user."
                });
            }

            bool deleted = await _serviceTaskService.DeleteServiceTaskAsync(
                id,
                deletedByUserId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Service task not found."
                });
            }

            return Ok(new
            {
                message = "Service task deleted successfully."
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
