using FieldStaff.Application.Features.Dashboard.DTOs;
using FieldStaff.Application.Features.SupervisorDashboard.DTOs;

namespace FieldStaff.Application.Features.Dashboard.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryResponseDto> GetAdminSummaryAsync();
        Task<SupervisorDashboardSummaryResponseDto> GetSupervisorSummaryAsync();
        Task<string?> GetRoleNameByIdAsync(int roleId);
    }
}