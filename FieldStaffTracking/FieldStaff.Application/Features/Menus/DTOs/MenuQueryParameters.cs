namespace FieldStaff.Application.Features.Menus.DTOs
{
    public class MenuQueryParameters
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public string SortBy { get; set; } = "menuName";

        public string SortDirection { get; set; } = "asc";

        public bool? IsActive { get; set; }

        public int? ParentMenuId { get; set; }
    }
}