
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class TaskStatusConfiguration : IEntityTypeConfiguration<StatusTask>
    {
        public void Configure(EntityTypeBuilder<StatusTask> builder)
        {
            builder.ToTable("TaskStatuses");

            builder.HasKey(s => s.TaskStatusId);

            builder.Property(s => s.TaskStatusName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.TaskStatusCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(s => s.TaskStatusCode)
                .IsUnique();
        }
    }
}
