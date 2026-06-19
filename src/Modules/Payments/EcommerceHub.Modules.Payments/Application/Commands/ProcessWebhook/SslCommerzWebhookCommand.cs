using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Payments.Application.Commands.ProcessWebhook;

/// <summary>
/// Represents an inbound SSLCommerz IPN payload.
/// Field names follow SSLCommerz IPN naming conventions; unrecognised fields are ignored.
/// </summary>
public sealed record SslCommerzWebhookCommand(
    string? TranId,
    string? ValId,
    string? Status,
    decimal Amount,
    string? Currency,
    string? BankTranId,
    string? StoreId) : IRequest<Result>;
