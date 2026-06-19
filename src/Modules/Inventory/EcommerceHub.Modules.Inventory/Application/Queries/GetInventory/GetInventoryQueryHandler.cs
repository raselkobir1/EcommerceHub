using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Modules.Inventory.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Inventory.Application.Queries.GetInventory;

internal sealed class GetInventoryQueryHandler(InventoryDbContext db)
    : IRequestHandler<GetInventoryQuery, Result<PagedResult<InventoryItemDto>>>
{
    private const int DefaultReorderPoint = 10;

    public async Task<Result<PagedResult<InventoryItemDto>>> Handle(
        GetInventoryQuery request, CancellationToken ct)
    {
        // Load all adjustments ordered by date; group by variant in memory.
        // EF Core cannot translate GroupBy + First() to SQL.
        var allAdjustments = await db.StockAdjustments
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

        // Latest adjustment per (ProductId, VariantId) = current stock snapshot
        var latest = allAdjustments
            .GroupBy(a => (a.ProductId, a.VariantId))
            .Select(g => g.First())
            .ToList();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            latest = latest
                .Where(a => a.ProductName.ToLower().Contains(term)
                         || a.Sku.ToLower().Contains(term))
                .ToList();
        }

        if (request.LowStock.HasValue)
        {
            latest = request.LowStock.Value
                ? latest.Where(a => a.NewStock <= DefaultReorderPoint).ToList()
                : latest.Where(a => a.NewStock > DefaultReorderPoint).ToList();
        }

        var total = latest.Count;
        var skip = (request.Page - 1) * request.PageSize;

        var items = latest
            .OrderBy(a => a.ProductName)
            .ThenBy(a => a.Sku)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(a => new InventoryItemDto(
                a.ProductId,
                a.VariantId,
                a.ProductName,
                a.Sku,
                StockQuantity: a.NewStock,
                ReservedQuantity: 0,
                AvailableQuantity: a.NewStock,
                DefaultReorderPoint,
                a.NewStock <= DefaultReorderPoint,
                a.CreatedAt))
            .ToList();

        return Result.Success(
            PagedResult<InventoryItemDto>.Create(items, total, request.Page, request.PageSize));
    }
}
