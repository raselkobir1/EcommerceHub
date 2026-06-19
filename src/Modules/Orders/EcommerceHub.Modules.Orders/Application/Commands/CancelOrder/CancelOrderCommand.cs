using MediatR;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Orders.Application.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId, Guid RequesterId)
    : IRequest<Result>;
