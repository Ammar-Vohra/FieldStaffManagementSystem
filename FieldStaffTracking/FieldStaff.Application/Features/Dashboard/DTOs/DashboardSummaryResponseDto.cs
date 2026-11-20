using FieldStaff.Application.Features.Dashboard.DTOs;

namespace FieldStaff.Application.Features.Dashboard.DTOs
{
    public class DashboardSummaryResponseDto
    {
        public int TotalTasks { get; set; }

        public int PendingTasks { get; set; }

        public int InProgressTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int TotalFieldStaff { get; set; }

        public int TotalCustomers { get; set; }

        public List<TaskStatusCountDto> TaskStatusDistribution { get; set; }
            = new();

        public List<DashboardTaskDto> TodaysTasks { get; set; }
            = new();

        public List<DashboardTaskDto> RecentTasks { get; set; }
            = new();

        public List<StaffWorkloadDto> StaffWorkload { get; set; }
            = new();
    }
}