using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RoleMenus.DTOs
{
    public class CreateRoleMenuRequestDto
    {
        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }

        [Range(1, int.MaxValue)]
        public int MenuId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
