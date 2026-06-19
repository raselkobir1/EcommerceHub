using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Modules.Inventory.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Inventory.Application.Queries.GetStockAdjustments;

internal sealed class GetStockAdjustmentsQueryHandler(InventoryDbContext db)
    : IRequestHandler<GetStockAdjustmentsQuery, Result<PagedResult<StockAdjustmentDto>>>
{
    public async Task<Result<PagedResult<StockAdjustmentDto>>> Handle(
        GetStockAdjustmentsQuery request, CancellationToken ct)
    {
        var query = db.StockAdjustments
            .AsNoTracking()
            .AsQueryable();

        if (request.ProductId.HasValue)
            query = query.Where(a => a.ProductId == request.ProductId.Value);

        if (request.VariantId.HasValue)
            query = query.Where(a => a.VariantId == request.VariantId.Value);

        if (request.From.HasValue)
            query = query.Where(a => a.CreatedAt >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(a => a.CreatedAt <= request.To.Value);

        var total = await query.CountAsync(ct);
        var skip = (request.Page - 1) * request.PageSize;

        var adjustments = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = adjustments
            .Select(a => new StockAdjustmentDto(
                a.Id,
                a.ProductId,
                a.VariantId,
                a.ProductName,
                a.Sku,
                a.QuantityChange,
                a.OldStock,
                a.NewStock,
                a.Reason.ToString(),
                a.Notes,
                a.AdjustedByUserId,
                a.CreatedAt))
            .ToList();

        return Result.Success(
            PagedResult<StockAdjustmentDto>.Create(dtos, total, request.Page, request.PageSize));
    }
}
