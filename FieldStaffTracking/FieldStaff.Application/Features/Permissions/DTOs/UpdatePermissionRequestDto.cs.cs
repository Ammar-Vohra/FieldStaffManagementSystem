
using System.ComponentModel.DataAnnotations;

namespace FieldStaff.Application.Features.Permissions.DTOs
{
    public class UpdatePermissionRequestDto
    {
        [Required]
        [StringLength(30)]
        public string PermissionName { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string PermissionCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
