using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.TaskStatuses.DTOs
{
    public class UpdateTaskStatusRequestDto
    {
        public string TaskStatusName { get; set; } = string.Empty;
        public string TaskStatusCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
