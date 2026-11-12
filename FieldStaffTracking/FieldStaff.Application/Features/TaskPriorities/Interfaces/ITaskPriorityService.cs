
using FieldStaff.Application.Features.TaskPriorities.DTOs;

namespace FieldStaff.Application.Features.TaskPriorities.Interfaces
{
    public interface ITaskPriorityService
    {
        Task<TaskPriorityResponseDto> CreateTaskPriorityAsync(
            CreateTaskPriorityRequestDto request,
            int? addedByUserId);

        Task<TaskPriorityResponseDto?> GetTaskPriorityByIdAsync(
            int taskPriorityId);

        Task<List<TaskPriorityResponseDto>> GetAllTaskPrioritiesAsync();

        Task<TaskPriorityResponseDto?> UpdateTaskPriorityAsync(
            int taskPriorityId,
            UpdateTaskPriorityRequestDto request,
            int? updatedByUserId);

        Task<bool> DeleteTaskPriorityAsync(
            int taskPriorityId,
            int? deletedByUserId);
    }
}
