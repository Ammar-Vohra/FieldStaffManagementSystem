namespace FieldStaff.Domain.Entities
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public Role Role { get; set; } = null!;

        public ICollection<ServiceTask> AssignedTasks { get; set; }
            = new List<ServiceTask>();

        public ICollection<TaskStatusHistory> TaskStatusHistories { get; set; }
            = new List<TaskStatusHistory>();

        public ICollection<UserPermission> UserPermissions { get; set; }
            = new List<UserPermission>();
    }
}