
namespace FieldStaff.Application.Features.Users.DTOs
{
    public class UserResponseDto
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
