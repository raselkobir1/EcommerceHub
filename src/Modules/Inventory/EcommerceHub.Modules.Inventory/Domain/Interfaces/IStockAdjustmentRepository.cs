using EcommerceHub.Modules.Inventory.Domain.Entities;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Inventory.Domain.Interfaces;

public interface IStockAdjustmentRepository : IRepository<StockAdjustment>
{
    Task<IEnumerable<StockAdjustment>> GetByProductAsync(Guid productId, CancellationToken ct = default);
    Task<IEnumerable<StockAdjustment>> GetByVariantAsync(Guid variantId, CancellationToken ct = default);
    Task<IEnumerable<StockAdjustment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
