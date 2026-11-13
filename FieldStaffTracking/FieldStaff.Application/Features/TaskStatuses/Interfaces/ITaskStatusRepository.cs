using FieldStaff.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.TaskStatuses.Interfaces
{
    public interface ITaskStatusRepository
    {
        Task<StatusTask?> GetByIdAsync(int taskStatusId);

        Task<List<StatusTask>> GetAllAsync();

        Task<StatusTask?> GetByCodeAsync(string taskStatusCode);

        Task AddAsync(StatusTask taskStatus);

        Task UpdateAsync(StatusTask taskStatus);

        Task<bool> ExistsByNameAsync(
            string taskStatusName,
            int? excludeTaskStatusId = null);

        Task<bool> ExistsByCodeAsync(
            string taskStatusCode,
            int? excludeTaskStatusId = null);
    }
}
