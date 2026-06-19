using MediatR;
using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Inventory.Application.Commands.AdjustStock;

public sealed record AdjustStockCommand(
    Guid ProductId, Guid VariantId,
    string ProductName, string Sku,
    int QuantityChange, int CurrentStock,
    string Reason, string? Notes,
    string AdjustedByUserId) : IRequest<Result<StockAdjustmentDto>>;
