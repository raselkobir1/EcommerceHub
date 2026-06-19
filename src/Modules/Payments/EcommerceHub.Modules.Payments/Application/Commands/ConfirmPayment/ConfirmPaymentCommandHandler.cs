using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using EcommerceHub.Shared.Kernel.Exceptions;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Commands.ConfirmPayment;

internal sealed class ConfirmPaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmPaymentCommand, Result>
{
    public async Task<Result> Handle(
        ConfirmPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        try
        {
            payment.MarkSuccess(
                gatewayTransactionId: request.TransactionId,
                gatewayReference: request.GatewayReference,
                gatewayResponse: request.GatewayResponse);
        }
        catch (EcommerceHub.Shared.Kernel.Exceptions.DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        paymentRepository.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
