using FieldStaff.Application.Features.TaskStatusHistories.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class TaskStatusHistoryRepository : ITaskStatusHistoryRepository
    {
        private readonly FieldStaffDbContext _context;

        public TaskStatusHistoryRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskStatusHistory>> GetByServiceTaskIdAsync(
            int serviceTaskId)
        {
            return await _context.TaskStatusHistories
                .AsNoTracking()
                .Include(h => h.OldStatus)
                .Include(h => h.NewStatus)
                .Include(h => h.User)
                .Where(h => h.ServiceTaskId == serviceTaskId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TaskStatusHistory history)
        {
            await _context.TaskStatusHistories.AddAsync(history);
        }
    }
}