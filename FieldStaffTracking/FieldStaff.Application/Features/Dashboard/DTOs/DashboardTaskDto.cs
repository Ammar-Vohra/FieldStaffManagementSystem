namespace FieldStaff.Application.Features.Dashboard.DTOs
{
    public class DashboardTaskDto
    {
        public int ServiceTaskId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string AssignedStaffName { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}