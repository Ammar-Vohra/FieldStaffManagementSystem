using FieldStaff.Application.Features.TaskStatuses.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.TaskStatuses.Interfaces
{
    public interface ITaskStatusService
    {
        Task<TaskStatusResponseDto> CreateTaskStatusAsync(
    CreateTaskStatusRequestDto request,
    int? addedByUserId);

        Task<TaskStatusResponseDto?> GetTaskStatusByIdAsync(
            int taskStatusId);

        Task<List<TaskStatusResponseDto>> GetAllTaskStatusesAsync();

        Task<TaskStatusResponseDto?> UpdateTaskStatusAsync(
            int taskStatusId,
            UpdateTaskStatusRequestDto request,
            int? updatedByUserId);

        Task<bool> DeleteTaskStatusAsync(
            int taskStatusId,
            int? deletedByUserId);
    }
}
