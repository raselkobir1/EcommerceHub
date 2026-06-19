using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ShortDescription,
    Guid CategoryId,
    Guid? BrandId,
    decimal BasePrice,
    decimal? CompareAtPrice,
    string? ThumbnailUrl,
    bool IsFeatured,
    int SortOrder,
    string? MetaTitle,
    string? MetaDescription) : IRequest<Result<ProductDetailDto>>;
