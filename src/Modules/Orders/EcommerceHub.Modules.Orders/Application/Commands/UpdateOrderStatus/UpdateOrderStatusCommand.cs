using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Orders.Application.Commands.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    string Status,
    string? Notes,
    string UpdatedBy) : IRequest<Result>;
