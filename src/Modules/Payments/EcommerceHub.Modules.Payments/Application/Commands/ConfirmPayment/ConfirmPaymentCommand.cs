using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Commands.ConfirmPayment;

public sealed record ConfirmPaymentCommand(
    Guid PaymentId,
    string TransactionId,
    string? GatewayReference = null,
    string? GatewayResponse = null) : IRequest<Result>;
