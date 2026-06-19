using EcommerceHub.Modules.Inventory.Domain.Entities;
using EcommerceHub.Modules.Inventory.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Inventory.Infrastructure.Persistence.Repositories;

internal sealed class StockAdjustmentRepository(InventoryDbContext context)
    : BaseRepository<StockAdjustment, InventoryDbContext>(context), IStockAdjustmentRepository
{
    public async Task<IEnumerable<StockAdjustment>> GetByProductAsync(Guid productId, CancellationToken ct = default)
        => await context.StockAdjustments.Where(s => s.ProductId == productId)
            .OrderByDescending(s => s.CreatedAt).ToListAsync(ct);

    public async Task<IEnumerable<StockAdjustment>> GetByVariantAsync(Guid variantId, CancellationToken ct = default)
        => await context.StockAdjustments.Where(s => s.VariantId == variantId)
            .OrderByDescending(s => s.CreatedAt).ToListAsync(ct);

    public async Task<IEnumerable<StockAdjustment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await context.StockAdjustments
            .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
            .OrderByDescending(s => s.CreatedAt).ToListAsync(ct);
}
