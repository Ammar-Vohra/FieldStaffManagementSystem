
using FieldStaff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldStaff.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.CustomerId);

            builder.Property(c => c.CustomerName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Email)
                .HasMaxLength(150);

            builder.Property(c => c.ServiceAddress)
                .IsRequired()
                .HasMaxLength(300);
        }
    }
}
