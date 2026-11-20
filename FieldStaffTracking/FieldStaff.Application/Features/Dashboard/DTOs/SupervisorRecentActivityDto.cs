namespace FieldStaff.Application.Features.SupervisorDashboard.DTOs
{
    public class SupervisorRecentActivityDto
    {
        public int TaskStatusHistoryId { get; set; }

        public int ServiceTaskId { get; set; }

        public string TaskTitle { get; set; } = string.Empty;

        public string? OldStatusName { get; set; }

        public string NewStatusName { get; set; } = string.Empty;

        public string UpdatedByName { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}