using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class BrandRepository(CatalogDbContext context)
    : BaseRepository<Brand, CatalogDbContext>(context), IBrandRepository
{
    public async Task<Brand?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await context.Brands.FirstOrDefaultAsync(b => b.Slug == slug, ct);

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default)
        => await context.Brands
            .AnyAsync(b => b.Slug == slug && (excludeId == null || b.Id != excludeId), ct);
}
