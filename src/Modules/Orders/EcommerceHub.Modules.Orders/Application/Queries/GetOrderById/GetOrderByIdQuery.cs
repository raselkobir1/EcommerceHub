using MediatR;
using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Orders.Application.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId, Guid? RequesterId)
    : IRequest<Result<OrderDto>>;
