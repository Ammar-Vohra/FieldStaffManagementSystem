using System.Collections.Generic;

namespace FieldStaff.Domain.Entities
{
    public class StatusTask : BaseEntity
    {
        public int TaskStatusId { get; set; }

        public string TaskStatusName { get; set; } = string.Empty;
        public string TaskStatusCode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public ICollection<ServiceTask> ServiceTasks { get; set; }
            = new List<ServiceTask>();

        public ICollection<TaskStatusHistory> OldStatusHistories { get; set; }
            = new List<TaskStatusHistory>();

        public ICollection<TaskStatusHistory> NewStatusHistories { get; set; }
            = new List<TaskStatusHistory>();
    }
}