using EcommerceHub.Modules.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(250).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(300).IsRequired();
        builder.Property(x => x.Sku).HasColumnName("sku").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ShortDescription).HasColumnName("short_description").HasMaxLength(500);
        builder.Property(x => x.FullDescription).HasColumnName("full_description");
        builder.Property(x => x.RegularPrice).HasColumnName("regular_price").HasColumnType("numeric(18,2)");
        builder.Property(x => x.SalePrice).HasColumnName("sale_price").HasColumnType("numeric(18,2)");
        builder.Property(x => x.IsVatApplicable).HasColumnName("is_vat_applicable").HasDefaultValue(false);
        builder.Property(x => x.CategoryId).HasColumnName("category_id");
        builder.Property(x => x.BrandId).HasColumnName("brand_id");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
        builder.Property(x => x.MetaTitle).HasColumnName("meta_title").HasMaxLength(160);
        builder.Property(x => x.MetaDescription).HasColumnName("meta_description").HasMaxLength(320);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("ix_products_slug");
        builder.HasIndex(x => x.Sku).IsUnique().HasDatabaseName("ix_products_sku");
        builder.HasIndex(x => x.CategoryId).HasDatabaseName("ix_products_category_id");
        builder.HasIndex(x => x.BrandId).HasDatabaseName("ix_products_brand_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_products_status");
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Images).WithOne().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Variants).WithOne().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Tags).WithOne().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.RelatedProducts).WithOne().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.ProductId).HasColumnName("product_id");
        builder.Property(x => x.Sku).HasColumnName("sku").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PriceOverride).HasColumnName("price_override").HasColumnType("numeric(18,2)");
        builder.Property(x => x.StockQuantity).HasColumnName("stock_quantity").HasDefaultValue(0);
        builder.Property(x => x.ReservedQuantity).HasColumnName("reserved_quantity").HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);

        builder.HasIndex(x => x.Sku).IsUnique().HasDatabaseName("ix_product_variants_sku");

        builder.HasMany(x => x.Attributes)
            .WithOne()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ProductVariantAttributeConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
{
    public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
    {
        builder.ToTable("product_variant_attributes");
        builder.HasKey(x => new { x.ProductVariantId, x.AttributeId, x.AttributeValueId });
        builder.Property(x => x.ProductVariantId).HasColumnName("product_variant_id");
        builder.Property(x => x.AttributeId).HasColumnName("attribute_id");
        builder.Property(x => x.AttributeValueId).HasColumnName("attribute_value_id");
    }
}

internal sealed class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.ToTable("product_tags");
        builder.HasKey(x => new { x.ProductId, x.TagId });
        builder.Property(x => x.ProductId).HasColumnName("product_id");
        builder.Property(x => x.TagId).HasColumnName("tag_id");
    }
}

internal sealed class ProductRelatedConfiguration : IEntityTypeConfiguration<ProductRelated>
{
    public void Configure(EntityTypeBuilder<ProductRelated> builder)
    {
        builder.ToTable("product_related");
        builder.HasKey(x => new { x.ProductId, x.RelatedProductId });
        builder.Property(x => x.ProductId).HasColumnName("product_id");
        builder.Property(x => x.RelatedProductId).HasColumnName("related_product_id");
    }
}

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.ProductId).HasColumnName("product_id");
        builder.Property(x => x.Url).HasColumnName("url").HasMaxLength(500).IsRequired();
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").HasDefaultValue(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
    }
}

internal sealed class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("product_attributes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.HasMany(x => x.Values).WithOne().HasForeignKey(x => x.AttributeId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("product_attribute_values");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.AttributeId).HasColumnName("attribute_id");
        builder.Property(x => x.Value).HasColumnName("value").HasMaxLength(100).IsRequired();
    }
}

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(120).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}

internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(120).IsRequired();
        builder.Property(x => x.LogoUrl).HasColumnName("logo_url").HasMaxLength(500);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
