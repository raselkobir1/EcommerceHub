namespace EcommerceHub.Modules.Payments.Application.DTOs;

public sealed record InitiatePaymentRequest(
    Guid OrderId,
    string OrderNumber,
    decimal Amount,
    string Method,
    string? CustomerPhone = null,
    string? CustomerEmail = null);

public sealed record InitiatePaymentResponseDto(
    Guid PaymentId,
    string Method,
    string Status,
    decimal Amount,
    string? RedirectUrl,
    string? TransactionId);

public sealed record PaymentStatusDto(
    Guid PaymentId,
    Guid OrderId,
    string OrderNumber,
    string Method,
    string Status,
    decimal Amount,
    string Currency,
    DateTime CreatedAt,
    DateTime? PaidAt,
    string? GatewayTransactionId,
    string? FailureReason,
    decimal RefundedAmount);

public sealed record ConfirmPaymentRequest(
    Guid PaymentId,
    string TransactionId);

public sealed record PaymentSummaryDto(
    Guid PaymentId,
    Guid OrderId,
    string OrderNumber,
    string Method,
    string Status,
    decimal Amount,
    DateTime CreatedAt,
    DateTime? PaidAt);
