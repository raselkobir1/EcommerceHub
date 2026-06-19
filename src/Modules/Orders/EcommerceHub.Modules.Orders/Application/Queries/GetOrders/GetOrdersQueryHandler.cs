using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Modules.Orders.Domain.Enums;
using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Orders.Application.Queries.GetOrders;

internal sealed class GetOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrdersQuery, Result<PagedResult<OrderListDto>>>
{
    public async Task<Result<PagedResult<OrderListDto>>> Handle(
        GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        IEnumerable<Domain.Entities.Order> orders;
        int totalCount;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var statusEnum))
                return Result.Failure<PagedResult<OrderListDto>>(
                    $"Invalid status filter '{request.Status}'.");

            totalCount = await orderRepository.CountByStatusAsync(statusEnum, cancellationToken);
            orders = await orderRepository.GetByStatusAsync(statusEnum, page, pageSize, cancellationToken);
        }
        else
        {
            var all = await orderRepository.GetAllAsync(cancellationToken);
            totalCount = all.Count;
            orders = all
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        var items = orders
            .Select(o => new OrderListDto(
                Id: o.Id,
                OrderNumber: o.OrderNumber,
                CustomerId: o.CustomerId,
                CustomerName: o.CustomerName,
                CustomerEmail: o.CustomerEmail,
                CustomerPhone: o.CustomerPhone,
                Status: o.Status.ToString(),
                PaymentStatus: o.PaymentStatus.ToString(),
                PaymentMethod: o.PaymentMethod,
                GrandTotal: o.GrandTotal,
                ItemCount: o.Items.Count,
                CreatedAt: o.CreatedAt))
            .ToList();

        return Result.Success(PagedResult<OrderListDto>.Create(items, totalCount, page, pageSize));
    }
}
