using MediatR;
using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Cart.Application.Queries.GetCart;

public sealed record GetCartQuery(Guid? CustomerId, string? SessionId) : IRequest<Result<CartDto>>;
