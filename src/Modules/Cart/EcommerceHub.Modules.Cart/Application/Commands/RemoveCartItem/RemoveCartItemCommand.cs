using MediatR;
using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Cart.Application.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(
    Guid? CustomerId, string? SessionId,
    Guid VariantId) : IRequest<Result<CartDto>>;
