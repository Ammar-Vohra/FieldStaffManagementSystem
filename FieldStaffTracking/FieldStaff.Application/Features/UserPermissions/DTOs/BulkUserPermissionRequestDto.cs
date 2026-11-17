
using System.ComponentModel.DataAnnotations;

namespace FieldStaff.Application.Features.UserPermissions.DTOs
{
    public class BulkUserPermissionRequestDto
    {
        [Required]
        public List<UserPermissionAssignmentDto> Permissions { get; set; }
            = new();
    }

    public class UserPermissionAssignmentDto
    {
        [Required]
        public int PermissionId { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
