using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.MenuPermissions.DTOs
{
    public class AssignMenuPermissionsRequestDto
    {
        [Required]
        [MinLength(1)]
        public List<int> PermissionIds { get; set; } = new();
    }
}
    