using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.TaskPriorities.DTOs
{
    public class TaskPriorityResponseDto
    {
        public int TaskPriorityId { get; set; }
        public string TaskPriorityName { get; set; } = string.Empty;
        public string TaskPriorityCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
