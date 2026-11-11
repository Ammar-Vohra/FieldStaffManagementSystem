
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menus");

            builder.HasKey(m => m.MenuId);

            builder.Property(m => m.MenuName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.MenuPath)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasMany(m => m.Permissions)
                .WithOne(p => p.Menu)
                .HasForeignKey(p => p.MenuId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
