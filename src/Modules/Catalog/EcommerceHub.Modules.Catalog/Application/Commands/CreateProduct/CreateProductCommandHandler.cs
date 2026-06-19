using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using EcommerceHub.Shared.Kernel.Exceptions;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IBrandRepository brandRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, Result<ProductDetailDto>>
{
    public async Task<Result<ProductDetailDto>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (!await categoryRepository.ExistsAsync(c => c.Id == request.CategoryId, ct))
            return Result.Failure<ProductDetailDto>($"Category '{request.CategoryId}' not found.");

        if (request.BrandId.HasValue && !await brandRepository.ExistsAsync(b => b.Id == request.BrandId.Value, ct))
            return Result.Failure<ProductDetailDto>($"Brand '{request.BrandId}' not found.");

        if (await productRepository.SlugExistsAsync(request.Slug, null, ct))
            return Result.Failure<ProductDetailDto>($"A product with slug '{request.Slug}' already exists.");

        // Generate SKU from slug
        var sku = request.Slug.ToUpper().Replace("-", "").Substring(0, Math.Min(8, request.Slug.Length));

        var product = Product.Create(
            request.Name,
            request.Slug,
            sku,
            request.BasePrice,
            request.CategoryId,
            request.ShortDescription,
            request.Description,
            request.BrandId);

        if (request.CompareAtPrice.HasValue)
            product.Update(request.Name, request.Slug, request.ShortDescription, request.Description,
                request.BasePrice, request.CompareAtPrice, request.CategoryId, request.BrandId, false, 0);

        product.SetSeo(request.MetaTitle, request.MetaDescription);

        foreach (var v in request.Variants)
        {
            var attrs = v.Attributes.Select(kvp =>
                (AttributeId: Guid.NewGuid(), AttributeValueId: Guid.NewGuid())).ToList();
            product.AddVariant(v.Sku, v.Price != request.BasePrice ? v.Price : null, v.StockQuantity, attrs);
        }

        await productRepository.AddAsync(product, ct);
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
            Enumerable.Empty<Guid>());
}
