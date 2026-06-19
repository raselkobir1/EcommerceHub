using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Slug,
    string? Description,
    string? ShortDescription,
    Guid CategoryId,
    Guid? BrandId,
    decimal BasePrice,
    decimal? CompareAtPrice,
    bool IsFeatured,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    IEnumerable<CreateVariantRequest> Variants,
    IEnumerable<string> Tags) : IRequest<Result<ProductDetailDto>>;

public sealed record CreateVariantRequest(
    string Sku,
    string? Name,
    decimal Price,
    decimal? CompareAtPrice,
    int StockQuantity,
    bool IsActive,
    IDictionary<string, string> Attributes);
