using FieldStaff.Application.Features.ServiceTasks.DTOs;

namespace FieldStaff.Application.Features.TaskAssignmentHistories.Interfaces
{
    public interface ITaskAssignmentHistoryService
    {
        Task<List<TaskAssignmentHistoryResponseDto>> GetByServiceTaskIdAsync(
            int serviceTaskId);
    }
}