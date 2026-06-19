using EcommerceHub.Modules.Reviews.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Reviews.Infrastructure.Persistence.Configurations;

internal sealed class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("product_reviews");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(x => x.Rating).HasColumnName("rating").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(x => x.Body).HasColumnName("body").HasColumnType("text");
        builder.Property(x => x.IsApproved).HasColumnName("is_approved").HasDefaultValue(false);
        builder.Property(x => x.IsVerifiedPurchase).HasColumnName("is_verified_purchase").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasIndex(x => x.ProductId).HasDatabaseName("ix_product_reviews_product_id");
        builder.HasIndex(x => x.CustomerId).HasDatabaseName("ix_product_reviews_customer_id");
        builder.HasIndex(x => new { x.ProductId, x.CustomerId, x.OrderId }).IsUnique()
            .HasDatabaseName("ix_product_reviews_product_customer_order");
    }
}
