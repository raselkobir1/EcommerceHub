using MediatR;
using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Cart.Application.Commands.UpdateCartItem;

public sealed record UpdateCartItemCommand(
    Guid? CustomerId, string? SessionId,
    Guid VariantId, int Quantity) : IRequest<Result<CartDto>>;
