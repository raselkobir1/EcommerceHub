using EcommerceHub.Modules.Payments.Application.DTOs;
using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using EcommerceHub.Shared.Kernel.Exceptions;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Queries.GetPaymentStatus;

internal sealed class GetPaymentStatusQueryHandler(IPaymentRepository paymentRepository)
    : IRequestHandler<GetPaymentStatusQuery, Result<PaymentStatusDto>>
{
    public async Task<Result<PaymentStatusDto>> Handle(
        GetPaymentStatusQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        var dto = new PaymentStatusDto(
            PaymentId: payment.Id,
            OrderId: payment.OrderId,
            OrderNumber: payment.OrderNumber,
            Method: payment.Method.ToString(),
            Status: payment.Status.ToString(),
            Amount: payment.Amount,
            Currency: payment.Currency,
            CreatedAt: payment.CreatedAt,
            PaidAt: payment.PaidAt,
            GatewayTransactionId: payment.GatewayTransactionId,
            FailureReason: payment.FailureReason,
            RefundedAmount: payment.RefundedAmount);

        return Result.Success(dto);
    }
}
