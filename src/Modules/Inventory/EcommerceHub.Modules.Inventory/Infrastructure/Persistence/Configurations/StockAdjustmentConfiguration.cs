using EcommerceHub.Modules.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Inventory.Infrastructure.Persistence.Configurations;

internal sealed class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("stock_adjustments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(x => x.VariantId).HasColumnName("variant_id").IsRequired();
        builder.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(300).IsRequired();
        builder.Property(x => x.Sku).HasColumnName("sku").HasMaxLength(100).IsRequired();
        builder.Property(x => x.QuantityChange).HasColumnName("quantity_change").IsRequired();
        builder.Property(x => x.OldStock).HasColumnName("old_stock").IsRequired();
        builder.Property(x => x.NewStock).HasColumnName("new_stock").IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasConversion<string>().IsRequired();
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(x => x.AdjustedByUserId).HasColumnName("adjusted_by_user_id").HasMaxLength(256).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasIndex(x => x.ProductId).HasDatabaseName("ix_stock_adjustments_product_id");
        builder.HasIndex(x => x.VariantId).HasDatabaseName("ix_stock_adjustments_variant_id");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_stock_adjustments_created_at");
    }
}
