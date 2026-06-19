using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Inventory.Application.Queries.GetAdjustments;

public sealed record GetAdjustmentsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? ProductId = null) : IRequest<Result<PagedResult<StockAdjustmentDto>>>;
