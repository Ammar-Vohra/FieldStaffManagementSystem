namespace FieldStaff.Application.Features.Dashboard.DTOs
{
    public class FieldStaffTaskDto
    {
        public int ServiceTaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string ServiceAddress { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;
        public string PriorityName { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? CompletionNote { get; set; }
    }
}