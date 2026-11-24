using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.RoleMenus.DTOs
{
    public class BulkRoleMenuAssignmentRequestDto
    {
        public List<int> MenuIds { get; set; } = new();
    }
}
