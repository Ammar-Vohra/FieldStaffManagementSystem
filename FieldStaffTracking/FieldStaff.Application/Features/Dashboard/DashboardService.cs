
using FieldStaff.Application.Features.Dashboard.DTOs;
using FieldStaff.Application.Features.Dashboard.Interfaces;
using FieldStaff.Application.Features.SupervisorDashboard.DTOs;

namespace FieldStaff.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

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

        
        public async Task<FieldStaffDashboardSummaryDto> GetFieldStaffSummaryAsync(int userId)
        {
            return await _dashboardRepository.GetFieldStaffSummaryAsync(userId);
        }

       
        public async Task<object?> GetSummaryForRoleAsync(int roleId, int userId)
        {
            var roleName = await _dashboardRepository.GetRoleNameByIdAsync(roleId);

            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            switch (roleName.Trim().ToLowerInvariant())
            {
                case "admin":
                    return await GetAdminSummaryAsync();

                case "supervisor":
                    return await GetSupervisorSummaryAsync();

                case "fieldstaff":
                    return await GetFieldStaffSummaryAsync(userId);

                default:
                    return null;
            }
        }
    }
}
