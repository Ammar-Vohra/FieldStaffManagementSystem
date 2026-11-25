
using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.ServiceTasks.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class ServiceTaskRepository : IServiceTaskRepository
    {
        private readonly FieldStaffDbContext _context;

        public ServiceTaskRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTask?> GetByIdAsync(int serviceTaskId)
        {
            return await _context.ServiceTasks
                .Include(t => t.Customer)
                .Include(t => t.AssignedStaff)
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskPriority)
                .FirstOrDefaultAsync(t =>
                    t.ServiceTaskId == serviceTaskId &&
                    !t.IsDeleted);
        }

        public async Task<StatusTask?> GetByCodeAsync(string taskStatusCode)
        {
            return await _context.TaskStatuses
                .FirstOrDefaultAsync(s =>
                    s.TaskStatusCode == taskStatusCode &&
                    s.IsActive &&
                    !s.IsDeleted);
        }

        public async Task<(List<ServiceTask> Tasks, int TotalRecords)> GetPagedAsync(
            ServiceTaskQueryParameters parameters)
        {
            IQueryable<ServiceTask> query = _context.ServiceTasks
                .AsNoTracking()
                .Include(t => t.Customer)
                .Include(t => t.AssignedStaff)
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskPriority)
                .Where(t => !t.IsDeleted);

            
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim();

                query = query.Where(t =>
                    EF.Functions.Like(t.Title, $"%{search}%") ||
                    (t.Description != null &&
                     EF.Functions.Like(t.Description, $"%{search}%")) ||
                    EF.Functions.Like(t.Customer.CustomerName, $"%{search}%") ||
                    EF.Functions.Like(t.AssignedStaff.UserName, $"%{search}%"));
            }

            
            if (parameters.CustomerId.HasValue)
            {
                query = query.Where(t =>
                    t.CustomerId == parameters.CustomerId.Value);
            }

            if (parameters.AssignedStaffId.HasValue)
            {
                query = query.Where(t =>
                    t.AssignedStaffId == parameters.AssignedStaffId.Value);
            }

            if (parameters.TaskStatusId.HasValue)
            {
                query = query.Where(t =>
                    t.TaskStatusId == parameters.TaskStatusId.Value);
            }

            if (parameters.TaskPriorityId.HasValue)
            {
                query = query.Where(t =>
                    t.TaskPriorityId == parameters.TaskPriorityId.Value);
            }

            if (parameters.ScheduledDateFrom.HasValue)
            {
                query = query.Where(t =>
                    t.ScheduledDate >= parameters.ScheduledDateFrom.Value);
            }

            if (parameters.ScheduledDateTo.HasValue)
            {
                DateTime endDate = parameters.ScheduledDateTo.Value.Date.AddDays(1);

                query = query.Where(t =>
                    t.ScheduledDate < endDate);
            }

            if (parameters.IsActive.HasValue)
            {
                query = query.Where(t =>
                    t.IsActive == parameters.IsActive.Value);
            }

            
            int totalRecords = await query.CountAsync();

            
            bool isDescending =
                parameters.SortDirection.Equals(
                    "desc",
                    StringComparison.OrdinalIgnoreCase);

            var orderedQuery = parameters.SortBy.ToLower() switch
            {
                "title" => isDescending
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),

                "customername" => isDescending
                    ? query.OrderByDescending(t => t.Customer.CustomerName)
                    : query.OrderBy(t => t.Customer.CustomerName),

                "assignedstaffname" => isDescending
                    ? query.OrderByDescending(t => t.AssignedStaff.UserName)
                    : query.OrderBy(t => t.AssignedStaff.UserName),

                "status" => isDescending
                    ? query.OrderByDescending(t => t.TaskStatus.TaskStatusName)
                    : query.OrderBy(t => t.TaskStatus.TaskStatusName),

                "priority" => isDescending
                    ? query.OrderByDescending(t => t.TaskPriority.TaskPriorityName)
                    : query.OrderBy(t => t.TaskPriority.TaskPriorityName),

                "scheduleddate" => isDescending
                    ? query.OrderByDescending(t => t.ScheduledDate)
                    : query.OrderBy(t => t.ScheduledDate),

                "isactive" => isDescending
                    ? query.OrderByDescending(t => t.IsActive)
                    : query.OrderBy(t => t.IsActive),

                "createdat" => isDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),

                _ => query.OrderByDescending(t => t.CreatedAt)
            };

            orderedQuery = orderedQuery.ThenBy(t => t.ServiceTaskId);

            
            var tasks = await orderedQuery
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (tasks, totalRecords);
        }


        public async Task<List<ServiceTask>> GetPendingTasksPastScheduledDateAsync(DateTime currentDateTime)
        {
            return await _context.ServiceTasks
                .Include(t => t.TaskStatus)
                .Where(t =>
                    !t.IsDeleted &&
                    t.IsActive &&
                    t.TaskStatus.TaskStatusCode == "PENDING" &&
                    t.ScheduledDate < currentDateTime)
                .ToListAsync();
        }

        public async Task<ServiceTask> CreateAsync(ServiceTask serviceTask)
        {
            await _context.ServiceTasks.AddAsync(serviceTask);
            return serviceTask;
        }

        public Task UpdateAsync(ServiceTask serviceTask)
        {
            _context.ServiceTasks.Update(serviceTask);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
