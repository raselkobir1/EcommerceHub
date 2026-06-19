using EcommerceHub.Modules.Promotions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Promotions.Infrastructure.Persistence.Configurations;

internal sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("coupons");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<string>().IsRequired();
        builder.Property(x => x.Value).HasColumnName("value").HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.MinimumOrderAmount).HasColumnName("minimum_order_amount").HasColumnType("numeric(18,2)");
        builder.Property(x => x.Scope).HasColumnName("scope").HasConversion<string>().IsRequired();
        builder.Property(x => x.TotalUsageLimit).HasColumnName("total_usage_limit");
        builder.Property(x => x.PerCustomerLimit).HasColumnName("per_customer_limit");
        builder.Property(x => x.UsedCount).HasColumnName("used_count").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(x => x.ExpiryDate).HasColumnName("expiry_date");
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("ix_coupons_code");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_coupons_is_active");

        builder.HasMany(x => x.ScopeItems)
            .WithOne()
            .HasForeignKey(x => x.CouponId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Usages)
            .WithOne()
            .HasForeignKey(x => x.CouponId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ScopeItems).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Usages).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class CouponScopeItemConfiguration : IEntityTypeConfiguration<CouponScopeItem>
{
    public void Configure(EntityTypeBuilder<CouponScopeItem> builder)
    {
        builder.ToTable("coupon_scope_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CouponId).HasColumnName("coupon_id").IsRequired();
        builder.Property(x => x.ReferenceId).HasColumnName("reference_id").IsRequired();
        builder.Property(x => x.Scope).HasColumnName("scope_type").HasConversion<string>().IsRequired();

        builder.HasIndex(x => x.CouponId).HasDatabaseName("ix_coupon_scope_items_coupon_id");
    }
}

internal sealed class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
{
    public void Configure(EntityTypeBuilder<CouponUsage> builder)
    {
        builder.ToTable("coupon_usages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CouponId).HasColumnName("coupon_id").IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id");
        builder.Property(x => x.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(x => x.UsedAt).HasColumnName("used_at").IsRequired();

        builder.HasIndex(x => x.CouponId).HasDatabaseName("ix_coupon_usages_coupon_id");
        builder.HasIndex(x => x.CustomerId).HasDatabaseName("ix_coupon_usages_customer_id");
        builder.HasIndex(x => x.OrderId).HasDatabaseName("ix_coupon_usages_order_id");
    }
}

internal sealed class FlashSaleConfiguration : IEntityTypeConfiguration<FlashSale>
{
    public void Configure(EntityTypeBuilder<FlashSale> builder)
    {
        builder.ToTable("flash_sales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(x => x.FlashPrice).HasColumnName("flash_price").HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.StartAt).HasColumnName("start_at").IsRequired();
        builder.Property(x => x.EndAt).HasColumnName("end_at").IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasIndex(x => x.ProductId).HasDatabaseName("ix_flash_sales_product_id");
        builder.HasIndex(x => new { x.StartAt, x.EndAt }).HasDatabaseName("ix_flash_sales_dates");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_flash_sales_is_active");
    }
}

internal sealed class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("banners");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Subtitle).HasColumnName("subtitle").HasMaxLength(300);
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(500).IsRequired();
        builder.Property(x => x.LinkUrl).HasColumnName("link_url").HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.ActiveFrom).HasColumnName("active_from");
        builder.Property(x => x.ActiveTo).HasColumnName("active_to");
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_banners_is_active");
        builder.HasIndex(x => x.SortOrder).HasDatabaseName("ix_banners_sort_order");
    }
}
