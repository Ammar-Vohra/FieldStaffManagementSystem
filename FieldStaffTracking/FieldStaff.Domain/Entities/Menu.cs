namespace FieldStaff.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;
        public string? MenuPath { get; set; }
        public string? Icon { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public Menu? ParentMenu { get; set; }

        public ICollection<MenuPermission> MenuPermissions { get; set; } = new List<MenuPermission>();
        public ICollection<Menu> ChildMenus { get; set; } = new List<Menu>();
    }
}