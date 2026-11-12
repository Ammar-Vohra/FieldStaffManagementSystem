
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.TaskPriorities.Interfaces
{
    public interface ITaskPriorityRepository
    {
        Task<TaskPriority?> GetByIdAsync(int taskPriorityId);

        Task<List<TaskPriority>> GetAllAsync();

        Task<TaskPriority?> GetByCodeAsync(string taskPriorityCode);

        Task AddAsync(TaskPriority taskPriority);

        Task UpdateAsync(TaskPriority taskPriority);

        Task<bool> ExistsByNameAsync(
            string taskPriorityName,
            int? excludeTaskPriorityId = null);

        Task<bool> ExistsByCodeAsync(
            string taskPriorityCode,
            int? excludeTaskPriorityId = null);
    }
}
