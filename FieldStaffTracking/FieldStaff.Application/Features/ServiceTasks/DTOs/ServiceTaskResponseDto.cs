
namespace FieldStaff.Application.Features.ServiceTasks.DTOs
    {
        public class ServiceTaskResponseDto
        {
            public int ServiceTaskId { get; set; }

            public string Title { get; set; } = string.Empty;

            public string? Description { get; set; }

            public int CustomerId { get; set; }

            public string CustomerName { get; set; } = string.Empty;

            public int AssignedStaffId { get; set; }

            public string AssignedStaffName { get; set; } = string.Empty;

            public int TaskStatusId { get; set; }

            public string TaskStatusName { get; set; } = string.Empty;

            public int TaskPriorityId { get; set; }

            public string TaskPriorityName { get; set; } = string.Empty;

            public DateTime ScheduledDate { get; set; }

            public string? CompletionNote { get; set; }

            public bool IsActive { get; set; }

            public DateTime CreatedAt { get; set; }
        }
    }


