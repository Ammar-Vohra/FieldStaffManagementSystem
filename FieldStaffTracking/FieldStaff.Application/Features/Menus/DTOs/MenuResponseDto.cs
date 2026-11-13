
namespace FieldStaff.Application.Features.Menus.DTOs
{
    public class MenuResponseDto
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public string? ParentMenuName { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? MenuPath { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public bool IsAcitve { get; set; }
    }
}
