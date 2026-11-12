using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FieldStaff.Application.Features.TaskPriorities.DTOs
{
    public class CreateTaskPriorityRequestDto
    {
        [Required]
        [StringLength(30)]
        public string TaskPriorityName { get; set; } = string.Empty;
        [Required]
        [StringLength(30)]
        public string TaskPriorityCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

    }
}
