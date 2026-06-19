using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EcommerceHub.Modules.Payments.Application.Commands.ProcessWebhook;

/// <summary>
/// Stub handler for bKash IPN callbacks.
/// Looks up the payment by gateway transaction ID and marks it as completed or failed
/// based on the transaction status received from bKash.
/// </summary>
internal sealed class BkashWebhookCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ILogger<BkashWebhookCommandHandler> logger)
    : IRequestHandler<BkashWebhookCommand, Result>
{
    private const string CompletedStatus = "Completed";

    public async Task<Result> Handle(BkashWebhookCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "bKash IPN received. PaymentID={PaymentID} TrxID={TrxID} Status={Status}",
            request.PaymentID, request.TrxID, request.TransactionStatus);

        if (string.IsNullOrWhiteSpace(request.TrxID))
            return Result.Failure("bKash IPN missing TrxID.");

        var payment = await paymentRepository.GetByGatewayTransactionIdAsync(request.TrxID, cancellationToken);

        if (payment is null)
        {
            // Payment not yet linked to TrxID; attempt lookup by MerchantInvoiceNumber (= our PaymentId).
            if (Guid.TryParse(request.MerchantInvoiceNumber, out var paymentId))
                payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        }

        if (payment is null)
        {
            logger.LogWarning("bKash IPN: payment not found for TrxID={TrxID}", request.TrxID);
            return Result.Failure($"Payment not found for TrxID '{request.TrxID}'.");
        }

        try
        {
            if (request.TransactionStatus == CompletedStatus)
                payment.MarkSuccess(request.TrxID, gatewayReference: request.PaymentID);
            else
                payment.MarkFailed($"bKash status: {request.TransactionStatus}");
        }
        catch (EcommerceHub.Shared.Kernel.Exceptions.DomainException ex)
        {
            logger.LogWarning(ex, "bKash IPN domain rule violation for TrxID={TrxID}", request.TrxID);
            return Result.Failure(ex.Message);
        }

        paymentRepository.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
