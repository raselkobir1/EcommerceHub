using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Inventory.Application.Queries.GetInventory;

public sealed record GetInventoryQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20,
    bool? LowStock = null) : IRequest<Result<PagedResult<InventoryItemDto>>>;
