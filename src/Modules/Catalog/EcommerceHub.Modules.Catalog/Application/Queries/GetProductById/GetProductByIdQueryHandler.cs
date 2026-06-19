using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Queries.GetProductById;

internal sealed class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IBrandRepository brandRepository)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDetailDto>>
{
    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await productRepository.GetWithDetailsAsync(request.ProductId, ct);
        if (product is null) return Result.Failure<ProductDetailDto>("Product not found.");

        var category = await categoryRepository.GetByIdAsync(product.CategoryId, ct);
        var brand = product.BrandId.HasValue ? await brandRepository.GetByIdAsync(product.BrandId.Value, ct) : null;

        return Result.Success(new ProductDetailDto(
            product.Id,
            product.Name,
            product.Slug,
            product.FullDescription,
            product.ShortDescription,
            product.Status.ToString(),
            product.RegularPrice,
            product.SalePrice,
            product.CategoryId,
            category?.Name ?? string.Empty,
            product.BrandId,
            brand?.Name,
            product.MetaTitle,
            product.MetaDescription,
            null,
            product.IsVisible,
            false,
            product.Variants.Select(v => new ProductVariantDto(
                v.Id, v.Sku, null,
                v.PriceOverride ?? product.RegularPrice,
                null, v.StockQuantity, v.ReservedQuantity, v.AvailableStock, v.IsActive,
                Enumerable.Empty<ProductVariantAttributeDto>())),
            product.Images.Select(i => new ProductImageDto(i.Id, i.Url, i.AltText, i.IsPrimary, i.SortOrder)),
            Enumerable.Empty<string>(),
            product.RelatedProducts.Select(r => r.RelatedProductId)));
    }
}
