using EcommerceHub.Modules.Notifications.Application.Services;
using EcommerceHub.Modules.Orders.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EcommerceHub.Modules.Notifications.Application.EventHandlers;

internal sealed class OrderConfirmedEmailHandler(IEmailService emailService, ILogger<OrderConfirmedEmailHandler> logger)
    : INotificationHandler<OrderConfirmedEvent>
{
    public async Task Handle(OrderConfirmedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Sending order confirmation email for order {OrderNumber}", notification.OrderNumber);
        await emailService.SendAsync(
            notification.CustomerEmail,
            $"Order Confirmed — {notification.OrderNumber}",
            $"<p>Your order <strong>{notification.OrderNumber}</strong> has been confirmed. Thank you for shopping with EcommerceHub!</p>",
            ct);
    }
}

internal sealed class OrderShippedEmailHandler(IEmailService emailService, ILogger<OrderShippedEmailHandler> logger)
    : INotificationHandler<OrderShippedEvent>
{
    public async Task Handle(OrderShippedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Sending shipping email for order {OrderNumber}", notification.OrderNumber);
        await emailService.SendAsync(
            notification.CustomerEmail,
            $"Your Order Has Shipped — {notification.OrderNumber}",
            $"<p>Your order <strong>{notification.OrderNumber}</strong> has been shipped. Tracking: <strong>{notification.TrackingNumber}</strong></p>",
            ct);
    }
}

internal sealed class OrderCancelledEmailHandler(IEmailService emailService)
    : INotificationHandler<OrderCancelledEvent>
{
    public async Task Handle(OrderCancelledEvent notification, CancellationToken ct) =>
        await emailService.SendAsync(
            notification.CustomerEmail,
            $"Order Cancelled — {notification.OrderNumber}",
            $"<p>Your order <strong>{notification.OrderNumber}</strong> has been cancelled. Reason: {notification.Reason}</p>",
            ct);
}
