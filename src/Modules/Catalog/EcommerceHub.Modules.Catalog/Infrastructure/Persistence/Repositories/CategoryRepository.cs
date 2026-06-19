using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository(CatalogDbContext context)
    : BaseRepository<Category, CatalogDbContext>(context), ICategoryRepository
{
    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await context.Categories.FirstOrDefaultAsync(c => c.Slug == slug, ct);

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken ct = default)
        => await context.Categories.Where(c => c.ParentId == null && c.IsActive)
            .Include(c => c.Children)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

    public async Task<IEnumerable<Category>> GetChildrenAsync(Guid parentId, CancellationToken ct = default)
        => await context.Categories.Where(c => c.ParentId == parentId && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default)
        => await context.Categories
            .AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId), ct);
}
