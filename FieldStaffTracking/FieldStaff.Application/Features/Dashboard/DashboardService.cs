using FieldStaff.Application.Features.Dashboard.DTOs;
using FieldStaff.Application.Features.Dashboard.Interfaces;
using FieldStaff.Application.Features.SupervisorDashboard.DTOs;

namespace FieldStaff.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public async Task<object?> GetSummaryForRoleAsync(int roleId)
        {
            var roleName = await _dashboardRepository.GetRoleNameByIdAsync(roleId);

            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            switch (roleName.Trim().ToLowerInvariant())
            {
                case "admin":
                    return await _dashboardRepository.GetAdminSummaryAsync();

                case "supervisor":
                    return await _dashboardRepository.GetSupervisorSummaryAsync();

                default:
                    return null;
            }
        }

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryResponseDto> GetAdminSummaryAsync()
        {
            return await _dashboardRepository.GetAdminSummaryAsync();
        }

        public async Task<SupervisorDashboardSummaryResponseDto> GetSupervisorSummaryAsync()
        {
            return await _dashboardRepository.GetSupervisorSummaryAsync();
        }
    }
}