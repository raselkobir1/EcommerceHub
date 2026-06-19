using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Orders.Application.Commands.PlaceOrder;

public sealed record PlaceOrderCommand(
    Guid? CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Division,
    string District,
    string AreaThana,
    string StreetAddress,
    string? ApartmentFloor,
    string PaymentMethod,
    string? CouponCode,
    IEnumerable<PlaceOrderItemRequest> Items) : IRequest<Result<OrderDto>>;
