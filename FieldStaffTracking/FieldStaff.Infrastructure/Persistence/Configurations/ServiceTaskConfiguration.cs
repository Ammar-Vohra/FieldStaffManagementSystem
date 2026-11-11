
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class ServiceTaskConfiguration : IEntityTypeConfiguration<ServiceTask>
    {
        public void Configure(EntityTypeBuilder<ServiceTask> builder)
        {
            builder.ToTable("ServiceTasks");

            builder.HasKey(t => t.ServiceTaskId);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.CompletionNote)
                .HasMaxLength(2000);

            builder.HasOne(t => t.Customer)
                .WithMany(c => c.ServiceTasks)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AssignedStaff)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedStaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TaskStatus)
                .WithMany(s => s.ServiceTasks)
                .HasForeignKey(t => t.TaskStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TaskPriority)
                .WithMany(p => p.ServiceTasks)
                .HasForeignKey(t => t.TaskPriorityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
