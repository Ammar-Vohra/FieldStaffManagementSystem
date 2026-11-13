using System.ComponentModel.DataAnnotations;
namespace FieldStaff.Application.Features.Permissions.DTOs
{
    public class CreatePermissionRequestDto
    {
        [Required]
        [StringLength(30)]
        public string PermissionName { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string PermissionCode { get; set; } = string.Empty;
    }
}
