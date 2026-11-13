using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.MenuPermissions.DTOs
{
    public class MenuPermissionResponseDto
    {
        public int MenuPermissionId { get; set; }

        public int MenuId { get; set; }

        public int PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

        public string PermissionCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
