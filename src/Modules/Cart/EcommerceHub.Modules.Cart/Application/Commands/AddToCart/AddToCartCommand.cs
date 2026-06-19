using MediatR;
using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Cart.Application.Commands.AddToCart;

public sealed record AddToCartCommand(
    Guid? CustomerId, string? SessionId,
    Guid ProductId, Guid VariantId,
    string ProductName, string Sku, string? ImageUrl,
    decimal UnitPrice, int Quantity) : IRequest<Result<CartDto>>;
