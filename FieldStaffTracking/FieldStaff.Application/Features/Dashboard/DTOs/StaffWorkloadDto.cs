namespace FieldStaff.Application.Features.Dashboard.DTOs
{
    public class StaffWorkloadDto
    {
        public int UserId { get; set; }

        public string StaffName { get; set; } = string.Empty;

        public int TotalAssignedTasks { get; set; }

        public int PendingTasks { get; set; }

        public int InProgressTasks { get; set; }

        public int CompletedTasks { get; set; }
    }
}