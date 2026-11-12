
using FieldStaff.Application.Features.TaskPriorities.DTOs;
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskPriority : ControllerBase
    {
        private readonly ITaskPriorityService _taskService;

        public TaskPriority(ITaskPriorityService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskPriority(
            [FromBody] CreateTaskPriorityRequestDto request)
        {
            try
            {
                int? createdByUserId = GetCurrentUserId();

                var result = await _taskService.CreateTaskPriorityAsync(
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
        public async Task<IActionResult> GetAllTaskPriorities()
        {
            var result = await _taskService.GetAllTaskPrioritiesAsync();

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
        public async Task<IActionResult> GetTaskPriorityById(int id)
        {
            var result = await _taskService.GetTaskPriorityByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Task priority not found."
                });
            }

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTaskPriority(
            int id,
            [FromBody] UpdateTaskPriorityRequestDto request)
        {
            try
            {
                int? updatedByUserId = GetCurrentUserId();

                var result = await _taskService.UpdateTaskPriorityAsync(
                    id,
                    request,
                    updatedByUserId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Task priority not found."
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

            bool deleted = await _taskService.DeleteTaskPriorityAsync(
                id,
                deletedByUserId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Task priority not found."
                });
            }

            return Ok(new
            {
                message = "Task priority deleted successfully."
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
