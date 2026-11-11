
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class TaskPriorityConfiguration : IEntityTypeConfiguration<TaskPriority>
    {
        public void Configure(EntityTypeBuilder<TaskPriority> builder)
        {
            builder.ToTable("TaskPriorities");

            builder.HasKey(p => p.TaskPriorityId);

            builder.Property(p => p.TaskPriorityName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.TaskPriorityCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.TaskPriorityCode)
                .IsUnique();
        }
    }
}
