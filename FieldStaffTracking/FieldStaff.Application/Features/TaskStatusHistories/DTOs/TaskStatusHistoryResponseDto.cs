namespace FieldStaff.Application.Features.ServiceTasks.DTOs
{
    public class TaskStatusHistoryResponseDto
    {
        public int TaskStatusHistoryId { get; set; }

        public int? OldStatusId { get; set; }

        public string? OldStatusName { get; set; }

        public int NewStatusId { get; set; }

        public string NewStatusName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}