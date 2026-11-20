using FieldStaff.Application.Features.Dashboard.DTOs;

namespace FieldStaff.Application.Features.SupervisorDashboard.DTOs
{
    public class SupervisorDashboardSummaryResponseDto
    {
        public int TotalTasks { get; set; }

        public int PendingTasks { get; set; }

        public int InProgressTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int OverdueTasks { get; set; }

        public int TotalFieldStaff { get; set; }

        public List<SupervisorTaskStatusCountDto> TaskStatusDistribution { get; set; }
            = new();

        public List<SupervisorDashboardTaskDto> TodaysTasks { get; set; }
            = new();

        public List<SupervisorDashboardTaskDto> RecentTasks { get; set; }
            = new();

        public List<SupervisorStaffWorkloadDto> StaffWorkload { get; set; }
            = new();

        public List<SupervisorRecentActivityDto> RecentActivities { get; set; }
            = new();
    }
}