namespace FieldStaff.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;
        public string MenuPath { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Permission> Permissions { get; set; }                        
            = new List<Permission>();
    }
}