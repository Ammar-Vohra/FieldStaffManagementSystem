using FieldStaff.Application.Features.Dashboard.DTOs;
using FieldStaff.Application.Features.SupervisorDashboard.DTOs;

namespace FieldStaff.Application.Features.Dashboard.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponseDto> GetAdminSummaryAsync();
        Task<SupervisorDashboardSummaryResponseDto> GetSupervisorSummaryAsync();
        Task<FieldStaffDashboardSummaryDto> GetFieldStaffSummaryAsync(int userId);
        Task<object?> GetSummaryForRoleAsync(int roleId, int userId);
    }
}