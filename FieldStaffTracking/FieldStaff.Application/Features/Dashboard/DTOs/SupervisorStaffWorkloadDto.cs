namespace FieldStaff.Application.Features.SupervisorDashboard.DTOs
{
    public class SupervisorStaffWorkloadDto
    {
        public int UserId { get; set; }

        public string StaffName { get; set; } = string.Empty;

        public int TotalAssignedTasks { get; set; }

        public int PendingTasks { get; set; }

        public int InProgressTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int OverdueTasks { get; set; }
    }
}