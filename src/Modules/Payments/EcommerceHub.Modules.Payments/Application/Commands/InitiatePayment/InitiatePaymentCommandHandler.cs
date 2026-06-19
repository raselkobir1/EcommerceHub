using EcommerceHub.Modules.Payments.Application.DTOs;
using EcommerceHub.Modules.Payments.Domain.Entities;
using EcommerceHub.Modules.Payments.Domain.Enums;
using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Commands.InitiatePayment;

internal sealed class InitiatePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<InitiatePaymentCommand, Result<InitiatePaymentResponseDto>>
{
    // SSLCommerz stub redirect base URL — replace with real gateway URL in production.
    private const string SslCommerzRedirectBase = "https://sslcommerz.example.com/pay";

    public async Task<Result<InitiatePaymentResponseDto>> Handle(
        InitiatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PaymentMethod>(request.Method, ignoreCase: true, out var method))
            return Result.Failure<InitiatePaymentResponseDto>(
                $"Unknown payment method '{request.Method}'.");

        var payment = Payment.Create(
            orderId: request.OrderId,
            orderNumber: request.OrderNumber,
            customerId: Guid.Empty, // resolved from order; caller may supply via command extension
            amount: request.Amount,
            method: method,
            ipAddress: request.IpAddress,
            customerPhone: request.CustomerPhone,
            customerEmail: request.CustomerEmail);

        await paymentRepository.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        string? redirectUrl = method switch
        {
            PaymentMethod.Cod => null,
            PaymentMethod.SslCommerz => $"{SslCommerzRedirectBase}?paymentId={payment.Id}",
            PaymentMethod.BKash => null,   // bKash uses in-app SDK; no redirect in this stub
            PaymentMethod.Nagad => null,   // Nagad uses in-app SDK; no redirect in this stub
            _ => null
        };

        var dto = new InitiatePaymentResponseDto(
            PaymentId: payment.Id,
            Method: payment.Method.ToString(),
            Status: payment.Status.ToString(),
            Amount: payment.Amount,
            RedirectUrl: redirectUrl,
            TransactionId: payment.GatewayTransactionId);

        return Result.Success(dto);
    }
}
