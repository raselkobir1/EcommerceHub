using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Orders.Application.Queries.GetOrders;

public sealed record GetOrdersQuery(
    string? Status,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<OrderListDto>>>;
