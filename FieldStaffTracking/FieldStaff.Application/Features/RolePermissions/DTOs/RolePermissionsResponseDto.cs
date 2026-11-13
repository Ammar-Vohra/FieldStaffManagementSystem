using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RolePermissions.DTOs
{
    public class RolePermissionsResponseDto
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public List<RolePermissionResponseDto> Permissions { get; set; }
            = new();
    }
}
