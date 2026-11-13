using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace FieldStaff.Application.Features.TaskStatuses.DTOs
{
    public class CreateTaskStatusRequestDto
    {
        [Required]
        [StringLength(20)]

        public string TaskStatusName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string TaskStatusCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
