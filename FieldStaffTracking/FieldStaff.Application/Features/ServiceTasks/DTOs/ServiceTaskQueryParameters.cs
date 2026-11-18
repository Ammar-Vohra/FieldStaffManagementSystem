
namespace FieldStaff.Application.Features.ServiceTasks.DTOs
    {
        public class ServiceTaskQueryParameters
        {
            public int PageNumber { get; set; } = 1;

            public int PageSize { get; set; } = 10;

            public string? Search { get; set; }

            public string? SortBy { get; set; }

            public string SortDirection { get; set; } = "asc";

            public int? CustomerId { get; set; }

            public int? AssignedStaffId { get; set; }

            public int? TaskStatusId { get; set; }

            public int? TaskPriorityId { get; set; }

            public DateTime? ScheduledDateFrom { get; set; }

            public DateTime? ScheduledDateTo { get; set; }

            public bool? IsActive { get; set; }
        }
    }

