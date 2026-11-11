
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.PermissionId);

            builder.Property(p => p.PermissionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PermissionCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.PermissionCode)
                .IsUnique();
        }
    }
}
