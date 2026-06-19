using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EcommerceHub.Modules.Payments.Application.Commands.ProcessWebhook;

/// <summary>
/// Stub handler for SSLCommerz IPN callbacks.
/// SSLCommerz sends a VALID/FAILED status in the "status" field.
/// The TranId field carries our internal payment ID (set during payment initiation).
/// </summary>
internal sealed class SslCommerzWebhookCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ILogger<SslCommerzWebhookCommandHandler> logger)
    : IRequestHandler<SslCommerzWebhookCommand, Result>
{
    private const string ValidStatus = "VALID";

    public async Task<Result> Handle(SslCommerzWebhookCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "SSLCommerz IPN received. TranId={TranId} ValId={ValId} Status={Status}",
            request.TranId, request.ValId, request.Status);

        if (!Guid.TryParse(request.TranId, out var paymentId))
        {
            logger.LogWarning("SSLCommerz IPN: TranId '{TranId}' is not a valid PaymentId GUID.", request.TranId);
            return Result.Failure($"TranId '{request.TranId}' is not a valid payment identifier.");
        }

        var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
        {
            logger.LogWarning("SSLCommerz IPN: payment {PaymentId} not found.", paymentId);
            return Result.Failure($"Payment '{paymentId}' not found.");
        }

        try
        {
            if (request.Status == ValidStatus)
            {
                payment.MarkSuccess(
                    gatewayTransactionId: request.BankTranId ?? request.ValId ?? request.TranId!,
                    gatewayReference: request.ValId);
            }
            else
            {
                payment.MarkFailed($"SSLCommerz status: {request.Status}");
            }
        }
        catch (EcommerceHub.Shared.Kernel.Exceptions.DomainException ex)
        {
            logger.LogWarning(ex, "SSLCommerz IPN domain rule violation for PaymentId={PaymentId}", paymentId);
            return Result.Failure(ex.Message);
        }

        paymentRepository.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
