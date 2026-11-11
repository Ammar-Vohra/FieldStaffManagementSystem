using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Domain.Entities
{
    public class TaskPriority : BaseEntity
    {
        public int TaskPriorityId { get; set; }
        public string TaskPriorityName { get; set; } = string.Empty;
        public string TaskPriorityCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;
        public ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();

    }
}
