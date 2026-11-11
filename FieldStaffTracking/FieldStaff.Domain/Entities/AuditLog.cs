namespace FieldStaff.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public int AuditLogId { get; set; }

        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;

        public int RecordId { get; set; }

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}
