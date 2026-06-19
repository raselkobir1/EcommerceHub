using MediatR;
using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Orders.Application.Queries.GetMyOrders;

internal sealed class GetMyOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetMyOrdersQuery, Result<PagedResult<OrderSummaryDto>>>
{
    public async Task<Result<PagedResult<OrderSummaryDto>>> Handle(
        GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        var dtos = orders
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryDto(
                Id: o.Id,
                OrderNumber: o.OrderNumber,
                CustomerName: o.CustomerName,
                Status: o.Status.ToString(),
                PaymentStatus: o.PaymentStatus.ToString(),
                GrandTotal: o.GrandTotal,
                ItemCount: o.Items.Count,
                CreatedAt: o.CreatedAt))
            .ToList();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Max(1, request.PageSize);
        var items = dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Result.Success(PagedResult<OrderSummaryDto>.Create(items, dtos.Count, page, pageSize));
    }
}
