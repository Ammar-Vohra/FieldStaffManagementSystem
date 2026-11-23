namespace FieldStaff.Application.Features.Dashboard.DTOs
{
    public class FieldStaffDashboardSummaryDto
    {
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }

        public List<FieldStaffTaskDto> TodaysTasks { get; set; } = new();
        public List<FieldStaffTaskDto> UpcomingTasks { get; set; } = new();
        public List<FieldStaffTaskDto> RecentCompletedTasks { get; set; } = new();
    }
}