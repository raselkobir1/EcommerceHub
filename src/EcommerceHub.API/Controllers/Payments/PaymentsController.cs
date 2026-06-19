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
    /// Returns gateway-specific data (redirect URL, token, etc.) needed by the client.
    /// </summary>
    [HttpPost("initiate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult InitiatePayment([FromBody] object request)
        => NotImplementedResponse();

    /// <summary>
    /// Webhook endpoint for bKash IPN (Instant Payment Notification) callbacks.
    /// Called directly by the bKash payment gateway — must remain anonymous.
    /// </summary>
    [HttpPost("webhook/bkash")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult BkashWebhook([FromBody] object payload)
        => NotImplementedResponse();

    /// <summary>
    /// Webhook endpoint for SSLCommerz IPN callbacks.
    /// Called directly by the SSLCommerz payment gateway — must remain anonymous.
    /// </summary>
    [HttpPost("webhook/sslcommerz")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult SslCommerzWebhook([FromBody] object payload)
        => NotImplementedResponse();

    /// <summary>Get all payment records associated with an order. Requires authentication.</summary>
    [HttpGet("order/{orderId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetPaymentsByOrder([FromRoute] Guid orderId)
        => NotImplementedResponse();
}
