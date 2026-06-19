namespace EcommerceHub.Modules.Catalog.Application.DTOs;

public sealed record CategoryDto(
    Guid Id, string Name, string Slug, string? Description, string? ImageUrl,
    Guid? ParentId, bool IsActive, int SortOrder, int ProductCount,
    IEnumerable<CategoryDto> Children);

public sealed record BrandDto(
    Guid Id, string Name, string Slug, string? Description, string? LogoUrl, bool IsActive);

public sealed record ProductVariantDto(
    Guid Id, string Sku, string? Name, decimal Price, decimal? CompareAtPrice,
    int StockQuantity, int ReservedQuantity, int AvailableStock, bool IsActive,
    IEnumerable<ProductVariantAttributeDto> Attributes);

public sealed record ProductVariantAttributeDto(string AttributeName, string AttributeValue);

public sealed record ProductImageDto(
    Guid Id, string Url, string? AltText, bool IsPrimary, int SortOrder);

public sealed record ProductListDto(
    Guid Id, string Name, string Slug, string? ShortDescription, string? PrimaryImageUrl,
    decimal MinPrice, decimal? MaxPrice, bool IsOnSale, bool IsVisible,
    string Status, string? BrandName, string CategoryName);

public sealed record ProductDetailDto(
    Guid Id, string Name, string Slug, string? Description, string? ShortDescription,
    string Status, decimal BasePrice, decimal? CompareAtPrice,
    Guid CategoryId, string CategoryName, Guid? BrandId, string? BrandName,
    string? MetaTitle, string? MetaDescription, string? MetaKeywords,
    bool IsVisible, bool IsFeatured,
    IEnumerable<ProductVariantDto> Variants,
    IEnumerable<ProductImageDto> Images,
    IEnumerable<string> Tags,
    IEnumerable<Guid> RelatedProductIds);
