
using FieldStaff.Application.Features.TaskStatuses.DTOs;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskStatus : ControllerBase
    {
        private readonly ITaskStatusService _taskService;

        public TaskStatus(ITaskStatusService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskStatus(
            [FromBody] CreateTaskStatusRequestDto request)
        {
            try
            {
                int? createdByUserId = GetCurrentUserId();

                var result = await _taskService.CreateTaskStatusAsync(
                    request,
                    createdByUserId);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTaskStatuses()
        {
            var result = await _taskService.GetAllTaskStatusesAsync();

            return Ok(result);
        }


        //// GET USERS WITH SEARCH, FILTER, SORT AND PAGINATION
        //[HttpGet]
        //public async Task<IActionResult> GetUsers(
        //    [FromQuery] UserQueryParameters parameters)
        //{
        //    try
        //    {
        //        var result = await _taskService.GetPagedTaskPrioritiesAsync(parameters);

        //        return Ok(result);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new
        //        {
        //            message = ex.Message
        //        });
        //    }
        //}


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTaskStatusById(int id)
        {
            var result = await _taskService.GetTaskStatusByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Task status not found."
                });
            }

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTaskStatus(
            int id,
            [FromBody] UpdateTaskStatusRequestDto request)
        {
            try
            {
                int? updatedByUserId = GetCurrentUserId();

                var result = await _taskService.UpdateTaskStatusAsync(
                    id,
                    request,
                    updatedByUserId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Task status not found."
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

            bool deleted = await _taskService.DeleteTaskStatusAsync(
                id,
                deletedByUserId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Task status not found."
                });
            }

            return Ok(new
            {
                message = "Task status deleted successfully."
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
