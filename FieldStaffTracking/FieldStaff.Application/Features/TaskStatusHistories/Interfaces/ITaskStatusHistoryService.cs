using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.TaskStatusHistories.Interfaces
{
    public interface ITaskStatusHistoryService
    {
        Task<List<TaskStatusHistoryResponseDto>> GetByServiceTaskIdAsync(
            int serviceTaskId);

        Task AddAsync(TaskStatusHistory history);
    }
}