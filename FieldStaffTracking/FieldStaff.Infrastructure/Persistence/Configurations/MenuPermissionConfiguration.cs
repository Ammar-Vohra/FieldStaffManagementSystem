
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class MenuPermissionConfiguration
        : IEntityTypeConfiguration<MenuPermission>
    {
        public void Configure(EntityTypeBuilder<MenuPermission> builder)
        {
            builder.ToTable("MenuPermissions");

            builder.HasKey(mp => mp.MenuPermissionId);

            builder.HasOne(mp => mp.Menu)
                .WithMany(m => m.MenuPermissions)
                .HasForeignKey(mp => mp.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mp => mp.Permission)
                .WithMany(p => p.MenuPermissions)
                .HasForeignKey(mp => mp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(mp => new
            {
                mp.MenuId,
                mp.PermissionId
            })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
