
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class TaskAssignmentHistoryConfiguration
        : IEntityTypeConfiguration<TaskAssignmentHistory>
    {
        public void Configure(EntityTypeBuilder<TaskAssignmentHistory> builder)
        {
            builder.ToTable("TaskAssignmentHistory");

            builder.HasKey(h => h.TaskAssignmentHistoryId);

            builder.Property(h => h.Remarks)
                .HasMaxLength(1000);


        

            builder.HasOne(h => h.ServiceTask)
                .WithMany(t => t.TaskAssignmentHistories)
                .HasForeignKey(h => h.ServiceTaskId)
                .OnDelete(DeleteBehavior.Restrict);


          

            builder.HasOne(h => h.OldAssignedStaff)
                .WithMany()
                .HasForeignKey(h => h.OldAssignedStaffId)
                .OnDelete(DeleteBehavior.Restrict);


            
            builder.HasOne(h => h.NewAssignedStaff)
                .WithMany()
                .HasForeignKey(h => h.NewAssignedStaffId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.HasQueryFilter(h => !h.ServiceTask.IsDeleted);
        }
    }
}
