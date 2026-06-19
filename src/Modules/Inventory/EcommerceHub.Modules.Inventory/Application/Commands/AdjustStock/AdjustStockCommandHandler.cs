using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Modules.Inventory.Domain.Entities;
using EcommerceHub.Modules.Inventory.Domain.Enums;
using EcommerceHub.Modules.Inventory.Domain.Interfaces;
using EcommerceHub.Modules.Inventory.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Inventory.Application.Commands.AdjustStock;

internal sealed class AdjustStockCommandHandler(
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

        StockAdjustment adjustment;
        try
        {
            adjustment = StockAdjustment.Create(
                request.ProductId,
                request.VariantId,
                request.ProductName,
                request.Sku,
                request.QuantityChange,
                request.CurrentStock,
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

        var dto = new StockAdjustmentDto(
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
            adjustment.CreatedAt);

        return Result.Success(dto);
    }
}
