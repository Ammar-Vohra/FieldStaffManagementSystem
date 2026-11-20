using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.TaskStatusHistories.Interfaces
{
    public interface ITaskStatusHistoryRepository
    {
        Task<List<TaskStatusHistory>> GetByServiceTaskIdAsync(int serviceTaskId);

        Task AddAsync(TaskStatusHistory history);
    }
}