namespace FieldStaff.Domain.Entities
{
    public class TaskAssignmentHistory : BaseEntity
    {
        public int TaskAssignmentHistoryId { get; set; }

        public int ServiceTaskId { get; set; }

        public int? OldAssignedStaffId { get; set; }

        public int NewAssignedStaffId { get; set; }

        public string? Remarks { get; set; }

        public ServiceTask ServiceTask { get; set; } = null!;

        public User? OldAssignedStaff { get; set; }

        public User NewAssignedStaff { get; set; } = null!;
    }
}