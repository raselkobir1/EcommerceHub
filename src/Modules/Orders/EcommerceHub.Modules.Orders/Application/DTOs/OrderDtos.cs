namespace EcommerceHub.Modules.Orders.Application.DTOs;

public sealed record OrderItemDto(
    Guid Id, Guid ProductId, Guid VariantId,
    string ProductName, string Sku, string? ImageUrl,
    decimal UnitPrice, int Quantity, decimal LineTotal,
    decimal? DiscountAmount);

public sealed record ShippingAddressDto(
    string Division, string District, string AreaThana,
    string StreetAddress, string? ApartmentFloor);

public sealed record OrderStatusHistoryDto(
    Guid Id, string Status, string? Note, DateTime ChangedAt, string? ChangedBy);

public sealed record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid? CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    ShippingAddressDto ShippingAddress,
    string Status,
    string PaymentStatus,
    string PaymentMethod,
    string? TrackingNumber,
    decimal SubTotal,
    decimal ShippingCharge,
    decimal DiscountAmount,
    decimal VatAmount,
    decimal GrandTotal,
    string? CouponCode,
    DateTime? EstimatedDeliveryDate,
    DateTime CreatedAt,
    IEnumerable<OrderItemDto> Items,
    IEnumerable<OrderStatusHistoryDto> StatusHistory);

public sealed record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    string Status,
    string PaymentStatus,
    decimal GrandTotal,
    int ItemCount,
    DateTime CreatedAt);

public sealed record PlaceOrderItemRequest(
    Guid ProductId,
    Guid VariantId,
    string ProductName,
    string Sku,
    string? ImageUrl,
    decimal UnitPrice,
    int Quantity);

public sealed record OrderListDto(
    Guid Id,
    string OrderNumber,
    Guid? CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Status,
    string PaymentStatus,
    string PaymentMethod,
    decimal GrandTotal,
    int ItemCount,
    DateTime CreatedAt);
