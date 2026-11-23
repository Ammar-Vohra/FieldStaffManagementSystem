using FieldStaff.Application.Features.Dashboard.DTOs;
using FieldStaff.Application.Features.Dashboard.Interfaces;
using FieldStaff.Application.Features.SupervisorDashboard.DTOs;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly FieldStaffDbContext _context;

        public DashboardRepository(FieldStaffDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetRoleNameByIdAsync(int roleId)
        {
            return await _context.Roles
                .Where(r => r.RoleId == roleId && r.IsActive)
                .Select(r => r.RoleName)
                .FirstOrDefaultAsync();
        }

        public async Task<DashboardSummaryResponseDto> GetAdminSummaryAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            
            var statuses = await _context.TaskStatuses
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.TaskStatusId,
                    s.TaskStatusName
                })
                .ToListAsync();

            var pendingStatusIds = statuses
                .Where(s => s.TaskStatusName.Trim()
                    .Equals("Pending", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            var inProgressStatusIds = statuses
                .Where(s => s.TaskStatusName.Trim()
                    .Equals("In Progress", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            var completedStatusIds = statuses
                .Where(s => s.TaskStatusName.Trim()
                    .Equals("Completed", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            
            var tasks = _context.ServiceTasks
                .Where(t => t.IsActive);

            var totalTasks = await tasks.CountAsync();

            var pendingTasks = await tasks
                .CountAsync(t => pendingStatusIds.Contains(t.TaskStatusId));

            var inProgressTasks = await tasks
                .CountAsync(t => inProgressStatusIds.Contains(t.TaskStatusId));

            var completedTasks = await tasks
                .CountAsync(t => completedStatusIds.Contains(t.TaskStatusId));

           
            var fieldStaff = _context.Users
                .Where(u =>
                    u.IsActive &&
                    u.Role.IsActive &&
                    u.Role.RoleName == "FieldStaff");

            var totalFieldStaff = await fieldStaff.CountAsync();

            
            var totalCustomers = await _context.Customers
                .CountAsync(c => c.IsActive);

            
            var taskStatusDistribution = await tasks
                .GroupBy(t => t.TaskStatus.TaskStatusName)
                .Select(g => new TaskStatusCountDto
                {
                    StatusName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

           
            var todaysTasks = await tasks
                .Where(t =>
                    t.ScheduledDate >= today &&
                    t.ScheduledDate < tomorrow)
                .OrderBy(t => t.ScheduledDate)
                .Select(t => new DashboardTaskDto
                {
                    ServiceTaskId = t.ServiceTaskId,
                    Title = t.Title,
                    CustomerName = t.Customer.CustomerName,
                    AssignedStaffName = t.AssignedStaff.UserName,
                    StatusName = t.TaskStatus.TaskStatusName,
                    ScheduledDate = t.ScheduledDate,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            var recentTasks = await tasks
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new DashboardTaskDto
                {
                    ServiceTaskId = t.ServiceTaskId,
                    Title = t.Title,
                    CustomerName = t.Customer.CustomerName,
                    AssignedStaffName = t.AssignedStaff.UserName,
                    StatusName = t.TaskStatus.TaskStatusName,
                    ScheduledDate = t.ScheduledDate,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            var staffWorkload = await fieldStaff
                .Select(u => new StaffWorkloadDto
                {
                    UserId = u.UserId,
                    StaffName = u.UserName,

                    TotalAssignedTasks = tasks
                        .Count(t => t.AssignedStaffId == u.UserId),

                    PendingTasks = tasks
                        .Count(t =>
                            t.AssignedStaffId == u.UserId &&
                            pendingStatusIds.Contains(t.TaskStatusId)),

                    InProgressTasks = tasks
                        .Count(t =>
                            t.AssignedStaffId == u.UserId &&
                            inProgressStatusIds.Contains(t.TaskStatusId)),

                    CompletedTasks = tasks
                        .Count(t =>
                            t.AssignedStaffId == u.UserId &&
                            completedStatusIds.Contains(t.TaskStatusId))
                })
                .ToListAsync();

            return new DashboardSummaryResponseDto
            {
                TotalTasks = totalTasks,
                PendingTasks = pendingTasks,
                InProgressTasks = inProgressTasks,
                CompletedTasks = completedTasks,
                TotalFieldStaff = totalFieldStaff,
                TotalCustomers = totalCustomers,
                TaskStatusDistribution = taskStatusDistribution,
                TodaysTasks = todaysTasks,
                RecentTasks = recentTasks,
                StaffWorkload = staffWorkload
            };
        }

        public async Task<SupervisorDashboardSummaryResponseDto> GetSupervisorSummaryAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Retrieve active task statuses dynamically.
            var statuses = await _context.TaskStatuses
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.TaskStatusId,
                    s.TaskStatusName
                })
                .ToListAsync();

            var pendingStatusIds = statuses
                .Where(s => s.TaskStatusName.Trim()
                    .Equals("Pending", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            var inProgressStatusIds = statuses
                .Where(s => s.TaskStatusName.Trim()
                    .Equals("In Progress", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            var completedStatusIds = statuses
                .Where(s => s.TaskStatusName.Trim()
                    .Equals("Completed", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            // All active tasks.
            var tasks = _context.ServiceTasks
                .Where(t => t.IsActive);

            // Summary counts.
            var totalTasks = await tasks.CountAsync();

            var pendingTasks = await tasks
                .CountAsync(t => pendingStatusIds.Contains(t.TaskStatusId));

            var inProgressTasks = await tasks
                .CountAsync(t => inProgressStatusIds.Contains(t.TaskStatusId));

            var completedTasks = await tasks
                .CountAsync(t => completedStatusIds.Contains(t.TaskStatusId));

            // Overdue means scheduled before today and not completed.
            var overdueTasks = await tasks
                .CountAsync(t =>
                    t.ScheduledDate < today &&
                    !completedStatusIds.Contains(t.TaskStatusId));

            // All active field staff, identified by RoleName.
            var fieldStaff = _context.Users
                .Where(u =>
                    u.IsActive &&
                    u.Role.IsActive &&
                    u.Role.RoleName == "FieldStaff");

            var totalFieldStaff = await fieldStaff.CountAsync();

            // Task status distribution.
            var taskStatusDistribution = await tasks
                .GroupBy(t => t.TaskStatus.TaskStatusName)
                .Select(g => new SupervisorTaskStatusCountDto
                {
                    StatusName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Today's tasks.
            var todaysTasks = await tasks
                .Where(t =>
                    t.ScheduledDate >= today &&
                    t.ScheduledDate < tomorrow)
                .OrderBy(t => t.ScheduledDate)
                .Select(t => new SupervisorDashboardTaskDto
                {
                    ServiceTaskId = t.ServiceTaskId,
                    Title = t.Title,
                    CustomerName = t.Customer.CustomerName,
                    AssignedStaffName = t.AssignedStaff.UserName,
                    StatusName = t.TaskStatus.TaskStatusName,
                    ScheduledDate = t.ScheduledDate,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            // Five most recently created tasks.
            var recentTasks = await tasks
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new SupervisorDashboardTaskDto
                {
                    ServiceTaskId = t.ServiceTaskId,
                    Title = t.Title,
                    CustomerName = t.Customer.CustomerName,
                    AssignedStaffName = t.AssignedStaff.UserName,
                    StatusName = t.TaskStatus.TaskStatusName,
                    ScheduledDate = t.ScheduledDate,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            // Staff workload.
            var staffWorkload = await fieldStaff
                .Select(u => new SupervisorStaffWorkloadDto
                {
                    UserId = u.UserId,
                    StaffName = u.UserName,

                    TotalAssignedTasks = tasks
                        .Count(t => t.AssignedStaffId == u.UserId),

                    PendingTasks = tasks
                        .Count(t =>
                            t.AssignedStaffId == u.UserId &&
                            pendingStatusIds.Contains(t.TaskStatusId)),

                    InProgressTasks = tasks
                        .Count(t =>
                            t.AssignedStaffId == u.UserId &&
                            inProgressStatusIds.Contains(t.TaskStatusId)),

                    CompletedTasks = tasks
                        .Count(t =>
                            t.AssignedStaffId == u.UserId &&
                            completedStatusIds.Contains(t.TaskStatusId)),

                    OverdueTasks = tasks
                        .Count(t =>
                            t.AssignedStaffId == u.UserId &&
                            t.ScheduledDate < today &&
                            !completedStatusIds.Contains(t.TaskStatusId))
                })
                .ToListAsync();



            // Five most recent task status changes.
            var recentActivities = await _context.TaskStatusHistories
                .Where(h => h.ServiceTask.IsActive)
                .OrderByDescending(h => h.CreatedAt)
                .Take(5)
                .Select(h => new SupervisorRecentActivityDto
                {
                    TaskStatusHistoryId = h.TaskStatusHistoryId,
                    ServiceTaskId = h.ServiceTaskId,
                    TaskTitle = h.ServiceTask.Title,
                    OldStatusName = h.OldStatus != null
                        ? h.OldStatus.TaskStatusName
                        : null,
                    NewStatusName = h.NewStatus.TaskStatusName,
                    UpdatedByName = h.User.UserName,
                    Remarks = h.Remarks,
                    UpdatedAt = h.CreatedAt
                })
                .ToListAsync();

            return new SupervisorDashboardSummaryResponseDto
            {
                TotalTasks = totalTasks,
                PendingTasks = pendingTasks,
                InProgressTasks = inProgressTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                TotalFieldStaff = totalFieldStaff,
                TaskStatusDistribution = taskStatusDistribution,
                TodaysTasks = todaysTasks,
                RecentTasks = recentTasks,
                StaffWorkload = staffWorkload,
                RecentActivities = recentActivities
            };
        }

        public async Task<FieldStaffDashboardSummaryDto> GetFieldStaffSummaryAsync(int userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var assignedTasks = _context.ServiceTasks
                .Where(t => t.IsActive && t.AssignedStaffId == userId);

            var statuses = await _context.TaskStatuses
                .Where(s => s.IsActive)
                .ToListAsync();

            var pendingStatusIds = statuses
                .Where(s => s.TaskStatusName.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            var inProgressStatusIds = statuses
                .Where(s => s.TaskStatusName.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            var completedStatusIds = statuses
                .Where(s => s.TaskStatusName.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.TaskStatusId)
                .ToList();

            var result = new FieldStaffDashboardSummaryDto
            {
                TotalTasks = await assignedTasks.CountAsync(),

                PendingTasks = await assignedTasks.CountAsync(
                    t => pendingStatusIds.Contains(t.TaskStatusId)),

                InProgressTasks = await assignedTasks.CountAsync(
                    t => inProgressStatusIds.Contains(t.TaskStatusId)),

                CompletedTasks = await assignedTasks.CountAsync(
                    t => completedStatusIds.Contains(t.TaskStatusId)),

                TodaysTasks = await assignedTasks
                    .Where(t => t.ScheduledDate >= today && t.ScheduledDate < tomorrow)
                    .OrderBy(t => t.ScheduledDate)
                    .Select(t => new FieldStaffTaskDto
                    {
                        ServiceTaskId = t.ServiceTaskId,
                        Title = t.Title,
                        Description = t.Description,
                        CustomerName = t.Customer.CustomerName,
                        ServiceAddress = t.Customer.ServiceAddress,
                        StatusName = t.TaskStatus.TaskStatusName,
                        PriorityName = t.TaskPriority.TaskPriorityName,
                        ScheduledDate = t.ScheduledDate,
                        CreatedAt = t.CreatedAt,
                        CompletionNote = t.CompletionNote
                    })
                    .ToListAsync(),

                UpcomingTasks = await assignedTasks
                    .Where(t => t.ScheduledDate >= tomorrow &&
                                !completedStatusIds.Contains(t.TaskStatusId))
                    .OrderBy(t => t.ScheduledDate)
                    .Take(10)
                    .Select(t => new FieldStaffTaskDto
                    {
                        ServiceTaskId = t.ServiceTaskId,
                        Title = t.Title,
                        Description = t.Description,
                        CustomerName = t.Customer.CustomerName,
                        ServiceAddress = t.Customer.ServiceAddress,
                        StatusName = t.TaskStatus.TaskStatusName,
                        PriorityName = t.TaskPriority.TaskPriorityName,
                        ScheduledDate = t.ScheduledDate,
                        CreatedAt = t.CreatedAt,
                        CompletionNote = t.CompletionNote
                    })
                    .ToListAsync(),

                RecentCompletedTasks = await assignedTasks
                    .Where(t => completedStatusIds.Contains(t.TaskStatusId))
                    .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                    .Take(5)
                    .Select(t => new FieldStaffTaskDto
                    {
                        ServiceTaskId = t.ServiceTaskId,
                        Title = t.Title,
                        Description = t.Description,
                        CustomerName = t.Customer.CustomerName,
                        ServiceAddress = t.Customer.ServiceAddress,
                        StatusName = t.TaskStatus.TaskStatusName,
                        PriorityName = t.TaskPriority.TaskPriorityName,
                        ScheduledDate = t.ScheduledDate,
                        CreatedAt = t.CreatedAt,
                        CompletionNote = t.CompletionNote
                    })
                    .ToListAsync()
            };

            return result;
        }


    }
}