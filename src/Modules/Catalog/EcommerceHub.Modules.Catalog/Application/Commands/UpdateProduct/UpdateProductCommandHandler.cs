using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IBrandRepository brandRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCommand, Result<ProductDetailDto>>
{
    public async Task<Result<ProductDetailDto>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetWithDetailsAsync(request.Id, ct);
        if (product is null)
            return Result.Failure<ProductDetailDto>($"Product '{request.Id}' not found.");

        if (!await categoryRepository.ExistsAsync(c => c.Id == request.CategoryId, ct))
            return Result.Failure<ProductDetailDto>($"Category '{request.CategoryId}' not found.");

        if (request.BrandId.HasValue && !await brandRepository.ExistsAsync(b => b.Id == request.BrandId.Value, ct))
            return Result.Failure<ProductDetailDto>($"Brand '{request.BrandId}' not found.");

        if (await productRepository.SlugExistsAsync(request.Slug, request.Id, ct))
            return Result.Failure<ProductDetailDto>($"A product with slug '{request.Slug}' already exists.");

        // CompareAtPrice maps to SalePrice — the domain requires SalePrice < RegularPrice
        decimal? salePrice = request.CompareAtPrice.HasValue && request.CompareAtPrice < request.BasePrice
            ? request.CompareAtPrice
            : null;

        product.Update(
            request.Name,
            request.Slug,
            request.ShortDescription,
            request.Description,
            request.BasePrice,
            salePrice,
            request.CategoryId,
            request.BrandId,
            false,
            request.SortOrder);

        product.SetSeo(request.MetaTitle, request.MetaDescription);

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, ct);
        Brand? brand = request.BrandId.HasValue
            ? await brandRepository.GetByIdAsync(request.BrandId.Value, ct)
            : null;

        return Result.Success(MapToDetailDto(product, category!.Name, brand?.Name));
    }

    private static ProductDetailDto MapToDetailDto(Product product, string categoryName, string? brandName)
        => new(
            product.Id,
            product.Name,
            product.Slug,
            product.FullDescription,
            product.ShortDescription,
            product.Status.ToString(),
            product.RegularPrice,
            product.SalePrice,
            product.CategoryId,
            categoryName,
            product.BrandId,
            brandName,
            product.MetaTitle,
            product.MetaDescription,
            null,
            product.IsVisible,
            false,
            product.Variants.Select(v => new ProductVariantDto(
                v.Id, v.Sku, null, v.PriceOverride ?? product.RegularPrice, null,
                v.StockQuantity, v.ReservedQuantity, v.AvailableStock, v.IsActive,
                Enumerable.Empty<ProductVariantAttributeDto>())),
            product.Images.Select(i => new ProductImageDto(i.Id, i.Url, i.AltText, i.IsPrimary, i.SortOrder)),
            Enumerable.Empty<string>(),
            product.RelatedProducts.Select(r => r.RelatedProductId));
}
