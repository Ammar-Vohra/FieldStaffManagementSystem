
namespace FieldStaff.Application.Features.Permissions.DTOs
{
    public class PermissionResponseDto
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

        public string PermissionCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
