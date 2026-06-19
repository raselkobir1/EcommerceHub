using MediatR;
using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;

namespace EcommerceHub.Modules.Orders.Application.Commands.CancelOrder;

internal sealed class CancelOrderCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    : IRequestHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Order not found.");

        if (order.CustomerId.HasValue && order.CustomerId.Value != request.RequesterId)
            return Result.Failure("Access denied.");

        try
        {
            order.Cancel("Cancelled by customer.", request.RequesterId.ToString());
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
