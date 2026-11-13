namespace FieldStaff.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public ICollection<MenuPermission> MenuPermissions { get; set; } = new List<MenuPermission>();
    }
    
}