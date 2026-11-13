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
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(m => m.Icon)
                .HasMaxLength(100);

            builder.HasOne(m => m.ParentMenu)
                .WithMany(m => m.ChildMenus)
                .HasForeignKey(m => m.ParentMenuId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => m.ParentMenuId);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}