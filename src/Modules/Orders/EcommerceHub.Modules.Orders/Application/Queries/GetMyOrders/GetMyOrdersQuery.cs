using MediatR;
using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
namespace EcommerceHub.Modules.Orders.Application.Queries.GetMyOrders;
public sealed record GetMyOrdersQuery(Guid CustomerId, int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<OrderSummaryDto>>>;
