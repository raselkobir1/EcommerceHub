using EcommerceHub.Modules.Orders.Domain.Enums;
using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Orders.Application.Commands.UpdateOrderStatus;

internal sealed class UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private static readonly string[] ValidStatuses =
    [
        nameof(OrderStatus.Confirmed),
        nameof(OrderStatus.Processing),
        nameof(OrderStatus.Shipped),
        nameof(OrderStatus.Delivered),
        nameof(OrderStatus.Cancelled),
        nameof(OrderStatus.ReturnRequested),
        nameof(OrderStatus.Returned)
    ];

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var targetStatus))
            return Result.Failure($"Invalid order status '{request.Status}'. Valid values: {string.Join(", ", ValidStatuses)}.");

        var order = await orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Order not found.");

        try
        {
            switch (targetStatus)
            {
                case OrderStatus.Processing:
                    order.MarkAsProcessing(request.UpdatedBy);
                    break;

                case OrderStatus.Shipped:
                    var trackingNumber = request.Notes ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(trackingNumber))
                        return Result.Failure("A tracking number must be provided in Notes when marking an order as Shipped.");
                    order.MarkAsShipped(trackingNumber, request.UpdatedBy);
                    break;

                case OrderStatus.Delivered:
                    order.MarkAsDelivered(request.UpdatedBy);
                    break;

                case OrderStatus.Cancelled:
                    order.Cancel(request.Notes ?? "Cancelled by admin.", request.UpdatedBy);
                    break;

                case OrderStatus.ReturnRequested:
                    order.RequestReturn(request.Notes ?? "Return requested by admin.");
                    break;

                case OrderStatus.Returned:
                    order.ConfirmReturn(request.UpdatedBy);
                    break;

                case OrderStatus.Confirmed:
                    return Result.Failure("Use the payment confirmation flow to set an order to 'Confirmed' status.");

                default:
                    return Result.Failure($"Transition to status '{request.Status}' is not supported via this endpoint.");
            }
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
