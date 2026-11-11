using System;
using System.Collections.Generic;

namespace FieldStaff.Domain.Entities
{
    public class ServiceTask : BaseEntity
    {
        public int ServiceTaskId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int CustomerId { get; set; }
        public int AssignedStaffId { get; set; }

        public int TaskStatusId { get; set; }
        public int TaskPriorityId { get; set; }

        public DateTime ScheduledDate { get; set; }
        public string? CompletionNote { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public Customer Customer { get; set; } = null!;
        public User AssignedStaff { get; set; } = null!;
        public StatusTask TaskStatus { get; set; } = null!;
        public TaskPriority TaskPriority { get; set; } = null!;

        public ICollection<TaskStatusHistory> TaskStatusHistories
        { get; set; } = new List<TaskStatusHistory>();
    }
}
