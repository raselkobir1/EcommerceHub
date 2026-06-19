using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Modules.Cart.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using CartEntity = EcommerceHub.Modules.Cart.Domain.Entities.Cart;

namespace EcommerceHub.Modules.Cart.Application.Queries.GetCart;

internal sealed class GetCartQueryHandler(ICartRepository cartRepository)
    : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken ct)
    {
        CartEntity? cart = null;

        if (request.CustomerId.HasValue)
            cart = await cartRepository.GetByCustomerIdAsync(request.CustomerId.Value, ct);
        else if (!string.IsNullOrWhiteSpace(request.SessionId))
            cart = await cartRepository.GetBySessionIdAsync(request.SessionId, ct);

        if (cart is null)
        {
            // Return an empty cart representation when none exists
            var emptyCart = new CartDto(
                Guid.Empty,
                request.CustomerId,
                request.SessionId,
                null,
                0m,
                0m,
                0,
                Enumerable.Empty<CartItemDto>());
            return Result.Success(emptyCart);
        }

        return Result.Success(MapToDto(cart));
    }

    internal static CartDto MapToDto(CartEntity cart) =>
        new(cart.Id,
            cart.CustomerId,
            cart.SessionId,
            cart.CouponCode,
            cart.CouponDiscount,
            cart.SubTotal,
            cart.TotalItems,
            cart.Items.Select(i => new CartItemDto(
                i.Id,
                i.ProductId,
                i.VariantId,
                i.ProductName,
                i.Sku,
                i.ImageUrl,
                i.UnitPrice,
                i.Quantity,
                i.LineTotal)));
}
