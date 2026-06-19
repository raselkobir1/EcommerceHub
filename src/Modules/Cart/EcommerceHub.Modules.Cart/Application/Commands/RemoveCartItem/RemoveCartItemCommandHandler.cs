using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Modules.Cart.Application.Queries.GetCart;
using EcommerceHub.Modules.Cart.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Cart.Application.Commands.RemoveCartItem;

internal sealed class RemoveCartItemCommandHandler(
    ICartRepository cartRepository,
    ICartUnitOfWork unitOfWork)
    : IRequestHandler<RemoveCartItemCommand, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        Domain.Entities.Cart? cart = null;

        if (request.CustomerId.HasValue)
            cart = await cartRepository.GetByCustomerIdAsync(request.CustomerId.Value, ct);
        else if (!string.IsNullOrWhiteSpace(request.SessionId))
            cart = await cartRepository.GetBySessionIdAsync(request.SessionId, ct);

        if (cart is null)
            return Result.Failure<CartDto>("Cart not found.");

        cart.RemoveItem(request.VariantId);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(GetCartQueryHandler.MapToDto(cart));
    }
}
