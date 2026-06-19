using EcommerceHub.Modules.Catalog.Domain.Enums;
using EcommerceHub.Modules.Catalog.Domain.Events;
using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Catalog.Domain.Entities;

public sealed class Product : SoftDeletableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Sku { get; private set; } = default!;
    public string? ShortDescription { get; private set; }
    public string? FullDescription { get; private set; }
    public decimal RegularPrice { get; private set; }
    public decimal? SalePrice { get; private set; }
    public bool IsVatApplicable { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? BrandId { get; private set; }
    public ProductStatus Status { get; private set; } = ProductStatus.Draft;
    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<ProductImage> _images = [];
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private readonly List<ProductVariant> _variants = [];
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    private readonly List<ProductTag> _tags = [];
    public IReadOnlyCollection<ProductTag> Tags => _tags.AsReadOnly();

    private readonly List<ProductRelated> _relatedProducts = [];
    public IReadOnlyCollection<ProductRelated> RelatedProducts => _relatedProducts.AsReadOnly();

    private Product() { }

    public static Product Create(
        string name, string slug, string sku,
        decimal regularPrice, Guid categoryId,
        string? shortDescription = null, string? fullDescription = null,
        Guid? brandId = null, bool isVatApplicable = false)
    {
        if (regularPrice < 0) throw new DomainException("Regular price cannot be negative.");

        var product = new Product
        {
            Name = name,
            Slug = slug.ToLowerInvariant(),
            Sku = sku.ToUpperInvariant(),
            ShortDescription = shortDescription,
            FullDescription = fullDescription,
            RegularPrice = regularPrice,
            CategoryId = categoryId,
            BrandId = brandId,
            IsVatApplicable = isVatApplicable
        };

        product.RaiseDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Sku));
        return product;
    }

    public void Update(string name, string slug, string? shortDescription,
        string? fullDescription, decimal regularPrice, decimal? salePrice,
        Guid categoryId, Guid? brandId, bool isVatApplicable, int sortOrder)
    {
        if (regularPrice < 0) throw new DomainException("Regular price cannot be negative.");
        if (salePrice.HasValue && salePrice >= regularPrice)
            throw new DomainException("Sale price must be less than regular price.");

        Name = name;
        Slug = slug.ToLowerInvariant();
        ShortDescription = shortDescription;
        FullDescription = fullDescription;
        RegularPrice = regularPrice;
        SalePrice = salePrice;
        CategoryId = categoryId;
        BrandId = brandId;
        IsVatApplicable = isVatApplicable;
        SortOrder = sortOrder;
    }

    public void SetSeo(string? metaTitle, string? metaDescription)
    {
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
    }

    public void Publish()
    {
        if (!_images.Any()) throw new DomainException("A product must have at least one image before publishing.");
        Status = ProductStatus.Active;
        RaiseDomainEvent(new ProductPublishedEvent(Id, Name));
    }

    public void Archive() => Status = ProductStatus.Archived;
    public void SetToDraft() => Status = ProductStatus.Draft;

    public void AddImage(string url, bool isPrimary, int sortOrder)
    {
        if (isPrimary) _images.ForEach(i => i.SetNotPrimary());
        _images.Add(ProductImage.Create(Id, url, isPrimary, sortOrder));
    }

    public void AddVariant(string sku, decimal? priceOverride, int stock,
        IEnumerable<(Guid AttributeId, Guid AttributeValueId)> attributes)
    {
        if (_variants.Any(v => v.Sku == sku.ToUpperInvariant()))
            throw new DomainException($"Variant with SKU '{sku}' already exists.");

        var variant = ProductVariant.Create(Id, sku, priceOverride, stock, attributes);
        _variants.Add(variant);
    }

    public void AddTag(Guid tagId)
    {
        if (!_tags.Any(t => t.TagId == tagId))
            _tags.Add(new ProductTag { ProductId = Id, TagId = tagId });
    }

    public void AddRelatedProduct(Guid relatedProductId)
    {
        if (_relatedProducts.Count >= 8)
            throw new DomainException("A product can have at most 8 related products.");
        if (!_relatedProducts.Any(r => r.RelatedProductId == relatedProductId))
            _relatedProducts.Add(new ProductRelated { ProductId = Id, RelatedProductId = relatedProductId });
    }

    public decimal EffectivePrice => SalePrice ?? RegularPrice;
    public bool IsOnSale => SalePrice.HasValue && SalePrice < RegularPrice;
    public bool IsVisible => Status == ProductStatus.Active && _variants.Any(v => v.StockQuantity > 0);
}
