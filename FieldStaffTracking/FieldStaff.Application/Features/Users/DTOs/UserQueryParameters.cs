
namespace FieldStaff.Application.Features.Users.DTOs
{
    public class UserQueryParameters
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public string SortBy { get; set; } = "userName";

        public string SortDirection { get; set; } = "asc";

        public bool? IsActive { get; set; }

        public int? RoleId { get; set; }
    }
}
