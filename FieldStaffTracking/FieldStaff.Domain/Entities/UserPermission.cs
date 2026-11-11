namespace FieldStaff.Domain.Entities
{
    public class UserPermission : BaseEntity
    {
        public int UserPermissionId { get; set; }

        public int UserId { get; set; }
        public int PermissionId { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}