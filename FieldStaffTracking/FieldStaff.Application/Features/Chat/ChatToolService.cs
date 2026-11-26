using FieldStaff.Application.Features.Chat.Interfaces;
using FieldStaff.Application.Features.Dashboard.Interfaces;
using FieldStaff.Application.Features.ServiceTasks.Interfaces;
using FieldStaff.Application.Features.ServiceTasks.DTOs;
using FieldStaff.Application.Features.TaskStatuses.Interfaces;

namespace FieldStaff.Application.Features.Chat
{
    public class ChatToolService : IChatToolService
    {
        private readonly IDashboardService _dashboardService;
        private readonly IServiceTaskService _serviceTaskService;
        private readonly ITaskStatusRepository _taskStatusRepository;

        public ChatToolService(
            IDashboardService dashboardService,
            IServiceTaskService serviceTaskService,
            ITaskStatusRepository taskStatusRepository)
        {
            _dashboardService = dashboardService;
            _serviceTaskService = serviceTaskService;
            _taskStatusRepository = taskStatusRepository;
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

            var result = await _serviceTaskService
                .GetPagedServiceTasksAsync(parameters);

            return result.Data;
        }
    }
}