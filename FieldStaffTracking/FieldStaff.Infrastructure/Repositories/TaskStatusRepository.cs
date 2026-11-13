
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class TaskStatusRepository : ITaskStatusRepository
    {
        private readonly FieldStaffDbContext _context;

        public TaskStatusRepository(FieldStaffDbContext context)
        {
            _context = context;
        }


        public async Task<StatusTask?> GetByIdAsync(int taskStatusId)
        {
            return await _context.TaskStatuses
                .FirstOrDefaultAsync(x =>
                    x.TaskStatusId == taskStatusId &&
                    !x.IsDeleted);
        }


        public async Task<List<StatusTask>> GetAllAsync()
        {
            return await _context.TaskStatuses
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TaskStatusId)
                .ToListAsync();
        }


        public async Task<StatusTask?> GetByCodeAsync(string taskStatusCode)
        {
            return await _context.TaskStatuses
                .FirstOrDefaultAsync(x =>
                    x.TaskStatusCode == taskStatusCode &&
                    !x.IsDeleted);
        }


        public async Task AddAsync(StatusTask taskStatus)
        {
            await _context.TaskStatuses.AddAsync(taskStatus);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(StatusTask taskStatus)
        {
            _context.TaskStatuses.Update(taskStatus);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> ExistsByNameAsync(
            string taskStatusName,
            int? excludeTaskStatusId = null)
        {
            return await _context.TaskStatuses.AnyAsync(x =>
                x.TaskStatusName == taskStatusName &&
                !x.IsDeleted &&
                (excludeTaskStatusId == null ||
                 x.TaskStatusId != excludeTaskStatusId));
        }


        public async Task<bool> ExistsByCodeAsync(
            string taskStatusCode,
            int? excludeTaskStatusId = null)
        {
            return await _context.TaskStatuses.AnyAsync(x =>
                x.TaskStatusCode == taskStatusCode &&
                !x.IsDeleted &&
                (excludeTaskStatusId == null ||
                 x.TaskStatusId != excludeTaskStatusId));
        }
    }
}
