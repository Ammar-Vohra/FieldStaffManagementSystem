
using FieldStaff.Application.Features.TaskPriorities.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class TaskPriorityRepository : ITaskPriorityRepository
    {
        private readonly FieldStaffDbContext _context;

        public TaskPriorityRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

       
        public async Task<TaskPriority?> GetByIdAsync(int taskPriorityId)
        {
            return await _context.TaskPriorities
                .FirstOrDefaultAsync(x =>
                    x.TaskPriorityId == taskPriorityId &&
                    !x.IsDeleted);
        }

     
        public async Task<List<TaskPriority>> GetAllAsync()
        {
            return await _context.TaskPriorities
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TaskPriorityId)
                .ToListAsync();
        }

       
        public async Task<TaskPriority?> GetByCodeAsync(string taskPriorityCode)
        {
            return await _context.TaskPriorities
                .FirstOrDefaultAsync(x =>
                    x.TaskPriorityCode == taskPriorityCode &&
                    !x.IsDeleted);
        }

       
        public async Task AddAsync(TaskPriority taskPriority)
        {
            await _context.TaskPriorities.AddAsync(taskPriority);
            await _context.SaveChangesAsync();
        }

      
        public async Task UpdateAsync(TaskPriority taskPriority)
        {
            _context.TaskPriorities.Update(taskPriority);
            await _context.SaveChangesAsync();
        }

       
        public async Task<bool> ExistsByNameAsync(
            string taskPriorityName,
            int? excludeTaskPriorityId = null)
        {
            return await _context.TaskPriorities.AnyAsync(x =>
                x.TaskPriorityName == taskPriorityName &&
                !x.IsDeleted &&
                (excludeTaskPriorityId == null ||
                 x.TaskPriorityId != excludeTaskPriorityId));
        }

        
        public async Task<bool> ExistsByCodeAsync(
            string taskPriorityCode,
            int? excludeTaskPriorityId = null)
        {
            return await _context.TaskPriorities.AnyAsync(x =>
                x.TaskPriorityCode == taskPriorityCode &&
                !x.IsDeleted &&
                (excludeTaskPriorityId == null ||
                 x.TaskPriorityId != excludeTaskPriorityId));
        }
    }
}
