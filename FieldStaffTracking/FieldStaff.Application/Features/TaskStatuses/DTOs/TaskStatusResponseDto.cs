using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.TaskStatuses.DTOs
{
    public class TaskStatusResponseDto
    {
        public int TaskStatusId { get; set; }
        public string TaskStatusName { get; set; } = string.Empty;
        public string TaskStatusCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
