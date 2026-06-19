using EcommerceHub.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Customers.Infrastructure.Persistence.Configurations;

internal sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("customer_addresses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.Label).HasColumnName("label").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Division).HasColumnName("division").HasMaxLength(100).IsRequired();
        builder.Property(x => x.District).HasColumnName("district").HasMaxLength(100).IsRequired();
        builder.Property(x => x.AreaThana).HasColumnName("area_thana").HasMaxLength(100).IsRequired();
        builder.Property(x => x.StreetAddress).HasColumnName("street_address").HasMaxLength(500).IsRequired();
        builder.Property(x => x.ApartmentFloor).HasColumnName("apartment_floor").HasMaxLength(100);
        builder.Property(x => x.IsDefault).HasColumnName("is_default").HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasIndex(x => x.CustomerId).HasDatabaseName("ix_customer_addresses_customer_id");
    }
}
