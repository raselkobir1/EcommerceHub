using EcommerceHub.Modules.Payments.Application.DTOs;
using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Queries.GetPaymentsByOrder;

internal sealed class GetPaymentsByOrderQueryHandler(IPaymentRepository paymentRepository)
    : IRequestHandler<GetPaymentsByOrderQuery, Result<IEnumerable<PaymentSummaryDto>>>
{
    public async Task<Result<IEnumerable<PaymentSummaryDto>>> Handle(
        GetPaymentsByOrderQuery request,
        CancellationToken cancellationToken)
    {
        // GetByOrderIdAsync returns the single active payment for the order.
        var payment = await paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);

        if (payment is null)
            return Result.Success(Enumerable.Empty<PaymentSummaryDto>());

        var dtos = new[]
        {
            new PaymentSummaryDto(
                PaymentId: payment.Id,
                OrderId: payment.OrderId,
                OrderNumber: payment.OrderNumber,
                Method: payment.Method.ToString(),
                Status: payment.Status.ToString(),
                Amount: payment.Amount,
                CreatedAt: payment.CreatedAt,
                PaidAt: payment.PaidAt)
        };

        return Result.Success<IEnumerable<PaymentSummaryDto>>(dtos);
    }
}
