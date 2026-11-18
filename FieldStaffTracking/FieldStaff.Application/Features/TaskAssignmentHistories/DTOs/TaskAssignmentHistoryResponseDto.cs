namespace FieldStaff.Application.Features.ServiceTasks.DTOs
{
    public class TaskAssignmentHistoryResponseDto
    {
        public int TaskAssignmentHistoryId { get; set; }

        public int? OldAssignedStaffId { get; set; }

        public string? OldAssignedStaffName { get; set; }

        public int NewAssignedStaffId { get; set; }

        public string NewAssignedStaffName { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}