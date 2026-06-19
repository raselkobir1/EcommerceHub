using EcommerceHub.Modules.Payments.Application.Commands.ConfirmPayment;
using EcommerceHub.Modules.Payments.Application.Commands.InitiatePayment;
using EcommerceHub.Modules.Payments.Application.Commands.ProcessWebhook;
using EcommerceHub.Modules.Payments.Application.DTOs;
using EcommerceHub.Modules.Payments.Application.Queries.GetPaymentsByOrder;
using EcommerceHub.Modules.Payments.Application.Queries.GetPaymentStatus;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Payments;

[Route("api/payments")]
public sealed class PaymentsController(MediatR.ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Initiate a payment for a placed order.
    /// For COD the response carries Status="Pending" and no redirect URL.
    /// For SSLCommerz a stub redirect URL is returned; for bKash/Nagad use the in-app SDK flow.
    /// </summary>
    [HttpPost("initiate")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<InitiatePaymentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InitiatePayment(
        [FromBody] InitiatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var command = new InitiatePaymentCommand(
            OrderId: request.OrderId,
            OrderNumber: request.OrderNumber,
            Amount: request.Amount,
            Method: request.Method,
            IpAddress: ipAddress,
            CustomerPhone: request.CustomerPhone,
            CustomerEmail: request.CustomerEmail);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Confirm a pending/processing payment (e.g., after COD delivery confirmation or
    /// manual gateway verification). Marks the payment as Completed.
    /// </summary>
    [HttpPost("confirm")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmPayment(
        [FromBody] ConfirmPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmPaymentCommand(
            PaymentId: request.PaymentId,
            TransactionId: request.TransactionId);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get the status of a specific payment by its ID.
    /// </summary>
    [HttpGet("{paymentId:guid}/status")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PaymentStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaymentStatus(
        [FromRoute] Guid paymentId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentStatusQuery(paymentId);
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all payment records associated with an order. Requires authentication.
    /// </summary>
    [HttpGet("order/{orderId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaymentsByOrder(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentsByOrderQuery(orderId);
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Webhook endpoint for bKash IPN (Instant Payment Notification) callbacks.
    /// Called directly by the bKash payment gateway — must remain anonymous.
    /// </summary>
    [HttpPost("webhook/bkash")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BkashWebhook(
        [FromBody] BkashIpnPayload payload,
        CancellationToken cancellationToken)
    {
        var command = new BkashWebhookCommand(
            PaymentID: payload.PaymentID,
            TrxID: payload.TrxID,
            TransactionStatus: payload.TransactionStatus,
            Amount: payload.Amount,
            Currency: payload.Currency,
            MerchantInvoiceNumber: payload.MerchantInvoiceNumber);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Webhook endpoint for SSLCommerz IPN callbacks.
    /// Called directly by the SSLCommerz payment gateway — must remain anonymous.
    /// </summary>
    [HttpPost("webhook/sslcommerz")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SslCommerzWebhook(
        [FromBody] SslCommerzIpnPayload payload,
        CancellationToken cancellationToken)
    {
        var command = new SslCommerzWebhookCommand(
            TranId: payload.TranId,
            ValId: payload.ValId,
            Status: payload.Status,
            Amount: payload.Amount,
            Currency: payload.Currency,
            BankTranId: payload.BankTranId,
            StoreId: payload.StoreId);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// IPN payload shapes — kept here to avoid an extra file for trivial records.
// ---------------------------------------------------------------------------

/// <summary>Minimal bKash IPN payload fields used by the webhook handler.</summary>
public sealed record BkashIpnPayload(
    string? PaymentID,
    string? TrxID,
    string? TransactionStatus,
    decimal Amount,
    string? Currency,
    string? MerchantInvoiceNumber);

/// <summary>Minimal SSLCommerz IPN payload fields used by the webhook handler.</summary>
public sealed record SslCommerzIpnPayload(
    string? TranId,
    string? ValId,
    string? Status,
    decimal Amount,
    string? Currency,
    string? BankTranId,
    string? StoreId);
