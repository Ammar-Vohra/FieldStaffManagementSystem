using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.TaskAssignmentHistories.Interfaces
{
    public interface ITaskAssignmentHistoryRepository
    {
        Task<List<TaskAssignmentHistory>> GetByServiceTaskIdAsync(
            int serviceTaskId);

        Task AddAsync(TaskAssignmentHistory history);
    }
}