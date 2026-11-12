
using System.ComponentModel.DataAnnotations;

namespace FieldStaff.Application.Features.Users.DTOs
{
    public class UpdateUserRequestDto
    {
        public int RoleId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
