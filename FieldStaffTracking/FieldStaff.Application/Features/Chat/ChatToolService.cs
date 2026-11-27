using FieldStaff.Application.Features.Chat.Interfaces;
using FieldStaff.Application.Features.Dashboard.Interfaces;
using FieldStaff.Application.Features.ServiceTasks.Interfaces;
using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;
using FieldStaff.Application.Features.Users.Interfaces;

namespace FieldStaff.Application.Features.Chat
{
    public class ChatToolService : IChatToolService
    {
        private readonly IDashboardService _dashboardService;
        private readonly IServiceTaskService _serviceTaskService;
        private readonly ITaskStatusRepository _taskStatusRepository;
        private readonly IUserRepository _userRepository;

        public ChatToolService(
            IDashboardService dashboardService,
            IServiceTaskService serviceTaskService,
            ITaskStatusRepository taskStatusRepository,
            IUserRepository userRepository)
        {
            _dashboardService = dashboardService;
            _serviceTaskService = serviceTaskService;
            _taskStatusRepository = taskStatusRepository;
            _userRepository = userRepository;
        }

        public async Task<object?> GetDashboardSummaryAsync(
            int roleId,
            int userId)
        {
            return await _dashboardService.GetSummaryForRoleAsync(
                roleId,
                userId);
        }

        public async Task<object?> GetOverdueTasksAsync(
            int roleId,
            int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || !user.IsActive || user.Role == null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var roleName = user.Role.RoleName;

            var overdueStatus = await _taskStatusRepository
                .GetByCodeAsync("OVERDUE");

            if (overdueStatus == null)
                return new { Message = "Overdue status was not found." };

            var parameters = new ServiceTaskQueryParameters
            {
                PageNumber = 1,
                PageSize = 100,
                TaskStatusId = overdueStatus.TaskStatusId,
                IsActive = true,
                SortBy = "scheduleddate",
                SortDirection = "asc"
            };

            if (roleName.Equals("FieldStaff", StringComparison.OrdinalIgnoreCase))
            {
                return (await _serviceTaskService.GetMyTasksAsync(userId, parameters)).Data;
            }

            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return (await _serviceTaskService.GetPagedServiceTasksAsync(parameters)).Data;
            }

            if (roleName.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                return (await _serviceTaskService.GetPagedServiceTasksAsync(parameters)).Data;
            }

            throw new UnauthorizedAccessException("Unknown user role");
        }

        public async Task<object?> GetServiceTaskAsync(int roleId, int userId, string? search)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || !user.IsActive || user.Role == null)
            {
                throw new UnauthorizedAccessException("User is not authorized.");
            }

            var roleName = user.Role.RoleName;

            var parameters = new ServiceTaskQueryParameters
            {
                PageNumber = 1,
                PageSize = 100,
                Search = search,
                IsActive = true,
                SortBy = "scheduleddate",
                SortDirection = "asc"
            };

            if (roleName.Equals("FieldStaff", StringComparison.OrdinalIgnoreCase))
            {
                return (await _serviceTaskService.GetMyTasksAsync(userId, parameters)).Data;
            }

            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                return (await _serviceTaskService.GetPagedServiceTasksAsync(parameters)).Data;
            }

            throw new UnauthorizedAccessException("Unknown user role");

        }
    }
}