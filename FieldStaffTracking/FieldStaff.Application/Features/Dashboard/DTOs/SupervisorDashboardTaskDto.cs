namespace FieldStaff.Application.Features.SupervisorDashboard.DTOs
{
    public class SupervisorDashboardTaskDto
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