using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.UserPermissions.DTOs
{
    public class CreateUserPermissionRequestDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PermissionId { get; set; }

        public bool IsGranted { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }
    }
}
