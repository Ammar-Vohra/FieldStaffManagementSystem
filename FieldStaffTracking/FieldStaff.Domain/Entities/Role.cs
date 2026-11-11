namespace FieldStaff.Domain.Entities
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public ICollection<User> Users { get; set; }
            = new List<User>();

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}