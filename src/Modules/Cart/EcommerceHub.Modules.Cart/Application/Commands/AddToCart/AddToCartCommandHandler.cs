using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Modules.Cart.Application.Queries.GetCart;
using EcommerceHub.Modules.Cart.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using CartEntity = EcommerceHub.Modules.Cart.Domain.Entities.Cart;

namespace EcommerceHub.Modules.Cart.Application.Commands.AddToCart;

internal sealed class AddToCartCommandHandler(
    ICartRepository cartRepository,
    ICartUnitOfWork unitOfWork)
    : IRequestHandler<AddToCartCommand, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(AddToCartCommand request, CancellationToken ct)
    {
        CartEntity? cart = null;

        if (request.CustomerId.HasValue)
            cart = await cartRepository.GetByCustomerIdAsync(request.CustomerId.Value, ct);
        else if (!string.IsNullOrWhiteSpace(request.SessionId))
            cart = await cartRepository.GetBySessionIdAsync(request.SessionId, ct);

        if (cart is null)
        {
            cart = request.CustomerId.HasValue
                ? CartEntity.CreateForCustomer(request.CustomerId.Value)
                : CartEntity.CreateForGuest(request.SessionId!);

            await cartRepository.AddAsync(cart, ct);
        }

        cart.AddItem(
            request.ProductId,
            request.VariantId,
            request.ProductName,
            request.Sku,
            request.ImageUrl,
            request.UnitPrice,
            request.Quantity);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(GetCartQueryHandler.MapToDto(cart));
    }
}
