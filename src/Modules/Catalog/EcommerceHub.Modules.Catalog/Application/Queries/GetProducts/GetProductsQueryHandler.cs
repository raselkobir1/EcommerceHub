using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Enums;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Modules.Catalog.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Catalog.Application.Queries.GetProducts;

internal sealed class GetProductsQueryHandler(CatalogDbContext db)
    : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductListDto>>>
{
    public async Task<Result<PagedResult<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var query = db.Products
            .Include(p => p.Images)
            .AsNoTracking()
            .AsQueryable();

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (request.BrandId.HasValue)
            query = query.Where(p => p.BrandId == request.BrandId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(p => p.Name.Contains(request.SearchTerm) ||
                                     p.Sku.Contains(request.SearchTerm));

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ProductStatus>(request.Status, true, out var status))
            query = query.Where(p => p.Status == status);

        var total = await query.CountAsync(ct);
        var skip = (request.Page - 1) * request.PageSize;

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = products.Select(p => new ProductListDto(
            p.Id, p.Name, p.Slug, p.ShortDescription,
            p.Images.FirstOrDefault(i => i.IsPrimary)?.Url,
            p.RegularPrice, p.SalePrice,
            p.IsOnSale, p.IsVisible,
            p.Status.ToString(), null, string.Empty)).ToList();

        return Result.Success(PagedResult<ProductListDto>.Create(dtos, total, request.Page, request.PageSize));
    }
}
