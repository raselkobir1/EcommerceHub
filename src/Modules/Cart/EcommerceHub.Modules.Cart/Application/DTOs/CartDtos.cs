namespace EcommerceHub.Modules.Cart.Application.DTOs;

public sealed record CartItemDto(
    Guid Id, Guid ProductId, Guid VariantId,
    string ProductName, string Sku, string? ImageUrl,
    decimal UnitPrice, int Quantity, decimal LineTotal);

public sealed record CartDto(
    Guid Id, Guid? CustomerId, string? SessionId,
    string? CouponCode, decimal CouponDiscount,
    decimal SubTotal, int TotalItems,
    IEnumerable<CartItemDto> Items);
