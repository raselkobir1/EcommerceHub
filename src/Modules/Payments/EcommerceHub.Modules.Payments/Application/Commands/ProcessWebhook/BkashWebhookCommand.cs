using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Commands.ProcessWebhook;

/// <summary>
/// Represents an inbound bKash IPN payload.
/// Field names match the bKash IPN specification; unused fields are ignored.
/// </summary>
public sealed record BkashWebhookCommand(
    string? PaymentID,
    string? TrxID,
    string? TransactionStatus,
    decimal Amount,
    string? Currency,
    string? MerchantInvoiceNumber) : IRequest<Result>;
