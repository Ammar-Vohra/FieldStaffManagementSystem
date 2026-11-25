using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.ServiceTasks.Interfaces
{
    public interface IServiceTaskRepository
    {
        Task<ServiceTask?> GetByIdAsync(int serviceTaskId);

        Task<(List<ServiceTask> Tasks, int TotalRecords)> GetPagedAsync(
            ServiceTaskQueryParameters parameters);
        Task<StatusTask?> GetByCodeAsync(string taskStatusCode);
        Task<List<ServiceTask>> GetPendingTasksPastScheduledDateAsync(DateTime currentDateTime);

        Task<ServiceTask> CreateAsync(ServiceTask serviceTask);

        Task UpdateAsync(ServiceTask serviceTask);

        Task SaveChangesAsync();
    }
}
