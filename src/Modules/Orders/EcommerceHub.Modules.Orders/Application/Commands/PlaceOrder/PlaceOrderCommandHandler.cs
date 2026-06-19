using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Modules.Orders.Domain.Entities;
using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Modules.Orders.Domain.ValueObjects;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Orders.Application.Commands.PlaceOrder;

internal sealed class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork)
    : IRequestHandler<PlaceOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var shippingAddress = ShippingAddress.Create(
            request.Division, request.District, request.AreaThana,
            request.StreetAddress, request.ApartmentFloor);

        var isGuest = !request.CustomerId.HasValue;

        var order = Order.Create(
            request.CustomerId,
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            shippingAddress,
            request.PaymentMethod,
            isGuest);

        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.VariantId, item.ProductName,
                item.Sku, item.ImageUrl, item.UnitPrice, item.Quantity);
        }

        await orderRepository.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDto(order));
    }

    private static OrderDto MapToDto(Order order) => new(
        order.Id,
        order.OrderNumber,
        order.CustomerId,
        order.CustomerName,
        order.CustomerEmail,
        order.CustomerPhone,
        new ShippingAddressDto(
            order.ShippingAddress.Division,
            order.ShippingAddress.District,
            order.ShippingAddress.AreaThana,
            order.ShippingAddress.StreetAddress,
            order.ShippingAddress.ApartmentFloor),
        order.Status.ToString(),
        order.PaymentStatus.ToString(),
        order.PaymentMethod,
        order.CourierTrackingNumber,
        order.SubTotal,
        order.ShippingCharge,
        order.DiscountAmount,
        order.VatAmount,
        order.GrandTotal,
        order.CouponCode,
        order.EstimatedDeliveryDate,
        order.CreatedAt,
        order.Items.Select(i => new OrderItemDto(
            i.Id, i.ProductId, i.VariantId, i.ProductName, i.Sku, i.ImageUrl,
            i.UnitPrice, i.Quantity, i.LineTotal, null)),
        order.StatusHistory.Select(h => new OrderStatusHistoryDto(
            h.Id, h.Status.ToString(), h.Note, h.ChangedAt, h.ChangedBy)));
}
