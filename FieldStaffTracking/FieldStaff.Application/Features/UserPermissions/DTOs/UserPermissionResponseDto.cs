using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldStaff.Application.Features.UserPermissions.DTOs
{
    public class UserPermissionResponseDto
    {
        public int UserPermissionId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;

        public bool IsGranted { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; }
    }
}
