using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.Roles.DTOs
{
    public class UpdateRoleRequestDto
    {
        public string? RoleName { get; set; } 
        public bool? IsActive { get; set; }
    }
}
