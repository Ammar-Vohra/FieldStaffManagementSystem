namespace FieldStaff.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;

        public int? MenuId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public Menu? Menu { get; set; }
    }
}