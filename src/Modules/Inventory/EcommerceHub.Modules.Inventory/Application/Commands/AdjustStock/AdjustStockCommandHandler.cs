using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Modules.Inventory.Domain.Entities;
using EcommerceHub.Modules.Inventory.Domain.Enums;
using EcommerceHub.Modules.Inventory.Domain.Interfaces;
using EcommerceHub.Modules.Inventory.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Inventory.Application.Commands.AdjustStock;

internal sealed class AdjustStockCommandHandler(
    InventoryDbContext db,
    IStockAdjustmentRepository stockAdjustmentRepository,
    IInventoryUnitOfWork unitOfWork)
    : IRequestHandler<AdjustStockCommand, Result<StockAdjustmentDto>>
{
    public async Task<Result<StockAdjustmentDto>> Handle(
        AdjustStockCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<AdjustmentReason>(request.Reason, ignoreCase: true, out var reason))
            return Result.Failure<StockAdjustmentDto>(
                $"Invalid adjustment reason '{request.Reason}'. " +
                $"Valid values: {string.Join(", ", Enum.GetNames<AdjustmentReason>())}.");

        // Look up the latest stock state for this variant
        var latest = await db.StockAdjustments
            .AsNoTracking()
            .Where(a => a.VariantId == request.VariantId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (latest is null)
            return Result.Failure<StockAdjustmentDto>(
                $"Variant '{request.VariantId}' has no inventory record. " +
                "Create an initial adjustment first.");

        StockAdjustment adjustment;
        try
        {
            adjustment = StockAdjustment.Create(
                latest.ProductId,
                latest.VariantId,
                latest.ProductName,
                latest.Sku,
                request.QuantityChange,
                latest.NewStock,
                reason,
                request.AdjustedByUserId,
                request.Notes);
        }
        catch (Exception ex)
        {
            return Result.Failure<StockAdjustmentDto>(ex.Message);
        }

        await stockAdjustmentRepository.AddAsync(adjustment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new StockAdjustmentDto(
            adjustment.Id,
            adjustment.ProductId,
            adjustment.VariantId,
            adjustment.ProductName,
            adjustment.Sku,
            adjustment.QuantityChange,
            adjustment.OldStock,
            adjustment.NewStock,
            adjustment.Reason.ToString(),
            adjustment.Notes,
            adjustment.AdjustedByUserId,
            adjustment.CreatedAt));
    }
}
