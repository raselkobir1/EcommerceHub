using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Inventory.Application.Queries.GetStockAdjustments;

/// <summary>
/// Returns a paginated list of stock adjustments, optionally filtered by product or variant
/// and bounded by a date range.
/// </summary>
public sealed record GetStockAdjustmentsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? ProductId = null,
    Guid? VariantId = null,
    DateTime? From = null,
    DateTime? To = null) : IRequest<Result<PagedResult<StockAdjustmentDto>>>;
