using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RolePermissions.DTOs
{
    public class RolePermissionResponseDto
    {
        public int RolePermissionId { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

        public string PermissionCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }

    }
}
