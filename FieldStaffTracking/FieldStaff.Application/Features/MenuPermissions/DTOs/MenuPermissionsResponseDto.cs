using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.MenuPermissions.DTOs
{
    public class MenuPermissionsResponseDto
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public List<MenuPermissionResponseDto> Permissions { get; set; }
            = new();
    }
}
