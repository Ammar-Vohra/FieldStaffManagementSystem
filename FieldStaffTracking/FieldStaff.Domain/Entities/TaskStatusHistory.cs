using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Domain.Entities
{
    public class TaskStatusHistory : BaseEntity
    {
        public int TaskStatusHistoryId { get; set; }
        public int ServiceTaskId { get; set; }
        public int? OldStatusId { get; set; }
        public int NewStatusId { get; set; }
        public string? Remarks { get; set; }
        public ServiceTask ServiceTask { get; set; } = null!;   
        public StatusTask? OldStatus { get; set; }
        public StatusTask NewStatus { get; set; } = null!;

    }
}
