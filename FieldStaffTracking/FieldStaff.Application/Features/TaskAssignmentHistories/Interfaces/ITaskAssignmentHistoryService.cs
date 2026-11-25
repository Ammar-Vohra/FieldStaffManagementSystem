using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.TaskAssignmentHistories.Interfaces
{
    public interface ITaskAssignmentHistoryService
    {
        Task<List<TaskAssignmentHistoryResponseDto>> GetByServiceTaskIdAsync(
            int serviceTaskId);
        Task AddAsync(TaskAssignmentHistory history);
    }
}
