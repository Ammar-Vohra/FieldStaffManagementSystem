
using System.ComponentModel.DataAnnotations;

namespace FieldStaff.Application.Features.Users.DTOs
{
    public class CreateUserRequestDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(30)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
