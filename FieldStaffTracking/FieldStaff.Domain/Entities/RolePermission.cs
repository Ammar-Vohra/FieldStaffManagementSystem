namespace FieldStaff.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public int RolePermissionId { get; set; }

        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}