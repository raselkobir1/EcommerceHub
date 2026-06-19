using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Enums;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Catalog.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken ct = default);
    Task<IEnumerable<Category>> GetChildrenAsync(Guid parentId, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);
}

public interface IBrandRepository : IRepository<Brand>
{
    Task<Brand?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);
}

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Product?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);
    Task<IEnumerable<Product>> SearchAsync(string term, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountByStatusAsync(ProductStatus status, CancellationToken ct = default);
}
