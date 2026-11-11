
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class TaskStatusHistoryConfiguration : IEntityTypeConfiguration<TaskStatusHistory>
    {
        public void Configure(EntityTypeBuilder<TaskStatusHistory> builder)
        {
            builder.ToTable("TaskStatusHistory");

            builder.HasKey(h => h.TaskStatusHistoryId);

            builder.Property(h => h.Remarks)
                .HasMaxLength(1000);

            builder.HasOne(h => h.ServiceTask)
                .WithMany(t => t.TaskStatusHistories)
                .HasForeignKey(h => h.ServiceTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.OldStatus)
                .WithMany(s => s.OldStatusHistories)
                .HasForeignKey(h => h.OldStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.NewStatus)
                .WithMany(s => s.NewStatusHistories)
                .HasForeignKey(h => h.NewStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
