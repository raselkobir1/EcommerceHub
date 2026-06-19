using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Enums;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(CatalogDbContext context)
    : BaseRepository<Product, CatalogDbContext>(context), IProductRepository
{
    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == slug, ct);

    public async Task<Product?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
        => await context.Products
            .Include(p => p.Variants).ThenInclude(v => v.Attributes)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.RelatedProducts)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
        => await context.Products
            .Include(p => p.Images)
            .Where(p => p.CategoryId == categoryId && p.Status == ProductStatus.Active)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId, CancellationToken ct = default)
        => await context.Products
            .Include(p => p.Images)
            .Where(p => p.BrandId == brandId && p.Status == ProductStatus.Active)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default)
        => await context.Products
            .AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId), ct);

    public async Task<IEnumerable<Product>> SearchAsync(string term, int page, int pageSize, CancellationToken ct = default)
        => await context.Products
            .Include(p => p.Images)
            .Where(p => p.Status == ProductStatus.Active &&
                        (p.Name.Contains(term) || (p.FullDescription != null && p.FullDescription.Contains(term))))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountByStatusAsync(ProductStatus status, CancellationToken ct = default)
        => await context.Products.CountAsync(p => p.Status == status, ct);
}
