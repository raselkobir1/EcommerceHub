using MediatR;
using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Orders.Application.Queries.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure<OrderDto>("Order not found.");

        if (request.RequesterId.HasValue && order.CustomerId != request.RequesterId.Value)
            return Result.Failure<OrderDto>("Access denied.");

        var shippingAddress = new ShippingAddressDto(
            order.ShippingAddress.Division,
            order.ShippingAddress.District,
            order.ShippingAddress.AreaThana,
            order.ShippingAddress.StreetAddress,
            order.ShippingAddress.ApartmentFloor);

        var items = order.Items.Select(i => new OrderItemDto(
            i.Id, i.ProductId, i.VariantId,
            i.ProductName, i.Sku, i.ImageUrl,
            i.UnitPrice, i.Quantity, i.LineTotal, null)).ToList();

        var statusHistory = order.StatusHistory.Select(h => new OrderStatusHistoryDto(
            h.Id, h.Status.ToString(), h.Note, h.ChangedAt, h.ChangedBy)).ToList();

        var dto = new OrderDto(
            order.Id, order.OrderNumber, order.CustomerId,
            order.CustomerName, order.CustomerEmail, order.CustomerPhone,
            shippingAddress,
            order.Status.ToString(), order.PaymentStatus.ToString(), order.PaymentMethod,
            order.CourierTrackingNumber,
            order.SubTotal, order.ShippingCharge, order.DiscountAmount,
            order.VatAmount, order.GrandTotal, order.CouponCode,
            order.EstimatedDeliveryDate, order.CreatedAt,
            items, statusHistory);

        return Result.Success(dto);
    }
}
