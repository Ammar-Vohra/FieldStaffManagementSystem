using FieldStaff.Application.Features.TaskAssignmentHistories.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class TaskAssignmentHistoryRepository
        : ITaskAssignmentHistoryRepository
    {
        private readonly FieldStaffDbContext _context;

        public TaskAssignmentHistoryRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskAssignmentHistory>>
            GetByServiceTaskIdAsync(int serviceTaskId)
        {
            return await _context.TaskAssignmentHistories
                .AsNoTracking()
                .Include(h => h.OldAssignedStaff)
                .Include(h => h.NewAssignedStaff)
                .Where(h => h.ServiceTaskId == serviceTaskId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TaskAssignmentHistory history)
        {
            await _context.TaskAssignmentHistories.AddAsync(history);
        }
    }
}