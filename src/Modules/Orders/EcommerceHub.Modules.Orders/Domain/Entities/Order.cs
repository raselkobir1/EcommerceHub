using EcommerceHub.Modules.Orders.Domain.Enums;
using EcommerceHub.Modules.Orders.Domain.Events;
using EcommerceHub.Modules.Orders.Domain.ValueObjects;
using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Orders.Domain.Entities;

public sealed class Order : AuditableEntity
{
    public string OrderNumber { get; private set; } = default!;
    public Guid? CustomerId { get; private set; }
    public string CustomerName { get; private set; } = default!;
    public string CustomerEmail { get; private set; } = default!;
    public string CustomerPhone { get; private set; } = default!;
    public ShippingAddress ShippingAddress { get; private set; } = default!;
    public OrderStatus Status { get; private set; } = OrderStatus.PendingPayment;
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;
    public string PaymentMethod { get; private set; } = default!;
    public string? GatewayTransactionId { get; private set; }
    public string? CourierTrackingNumber { get; private set; }
    public Guid? DeliveryZoneId { get; private set; }
    public string? ShippingMethod { get; private set; }
    public decimal ShippingCharge { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal VatAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public string? CouponCode { get; private set; }
    public string? InternalNotes { get; private set; }
    public bool IsGuest { get; private set; }
    public DateTime? EstimatedDeliveryDate { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private readonly List<OrderStatusHistory> _statusHistory = [];
    public IReadOnlyCollection<OrderStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    private Order() { }

    public static Order Create(
        Guid? customerId, string customerName, string customerEmail, string customerPhone,
        ShippingAddress shippingAddress, string paymentMethod, bool isGuest = false)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            ShippingAddress = shippingAddress,
            PaymentMethod = paymentMethod,
            IsGuest = isGuest
        };

        order._statusHistory.Add(OrderStatusHistory.Create(order.Id, OrderStatus.PendingPayment, "Order placed."));
        order.RaiseDomainEvent(new OrderPlacedEvent(order.Id, order.OrderNumber, order.CustomerEmail, order.GrandTotal));
        return order;
    }

    public void AddItem(Guid productId, Guid variantId, string productName, string sku,
        string? imageUrl, decimal unitPrice, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.VariantId == variantId);
        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
        }
        else
        {
            _items.Add(OrderItem.Create(Id, productId, variantId, productName, sku, imageUrl, unitPrice, quantity));
        }
        RecalculateTotals();
    }

    public void SetShipping(Guid deliveryZoneId, string shippingMethod, decimal shippingCharge, DateTime? estimatedDeliveryDate)
    {
        DeliveryZoneId = deliveryZoneId;
        ShippingMethod = shippingMethod;
        ShippingCharge = shippingCharge;
        EstimatedDeliveryDate = estimatedDeliveryDate;
        RecalculateTotals();
    }

    public void ApplyCoupon(string couponCode, decimal discountAmount)
    {
        CouponCode = couponCode;
        DiscountAmount = discountAmount;
        RecalculateTotals();
    }

    public void SetVat(decimal vatAmount)
    {
        VatAmount = vatAmount;
        RecalculateTotals();
    }

    public void ConfirmPayment(string transactionId)
    {
        if (PaymentStatus == PaymentStatus.Paid)
            throw new DomainException("Order is already paid.");

        GatewayTransactionId = transactionId;
        PaymentStatus = PaymentStatus.Paid;
        UpdateStatus(OrderStatus.Confirmed, "Payment confirmed via gateway.");
        RaiseDomainEvent(new OrderConfirmedEvent(Id, OrderNumber, CustomerEmail));
    }

    public void MarkAsProcessing(string updatedBy)
    {
        EnsureStatus(OrderStatus.Confirmed);
        UpdateStatus(OrderStatus.Processing, $"Order being packed by {updatedBy}.");
    }

    public void MarkAsShipped(string trackingNumber, string updatedBy)
    {
        EnsureStatus(OrderStatus.Processing);
        CourierTrackingNumber = trackingNumber;
        UpdateStatus(OrderStatus.Shipped, $"Shipped. Tracking: {trackingNumber}");
        RaiseDomainEvent(new OrderShippedEvent(Id, OrderNumber, CustomerEmail, trackingNumber));
    }

    public void MarkAsDelivered(string updatedBy)
    {
        EnsureStatus(OrderStatus.Shipped);
        UpdateStatus(OrderStatus.Delivered, $"Marked as delivered by {updatedBy}.");
        RaiseDomainEvent(new OrderDeliveredEvent(Id, OrderNumber, CustomerEmail));
    }

    public void Cancel(string reason, string cancelledBy)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Returned)
            throw new DomainException("Cannot cancel a delivered or returned order.");
        UpdateStatus(OrderStatus.Cancelled, $"Cancelled by {cancelledBy}: {reason}");
        RaiseDomainEvent(new OrderCancelledEvent(Id, OrderNumber, CustomerEmail, reason));
    }

    public void RequestReturn(string reason)
    {
        EnsureStatus(OrderStatus.Delivered);
        UpdateStatus(OrderStatus.ReturnRequested, $"Return requested: {reason}");
    }

    public void ConfirmReturn(string updatedBy)
    {
        EnsureStatus(OrderStatus.ReturnRequested);
        UpdateStatus(OrderStatus.Returned, $"Return confirmed by {updatedBy}.");
        PaymentStatus = PaymentStatus.Refunded;
    }

    public void AddInternalNote(string note) => InternalNotes = note;

    private void UpdateStatus(OrderStatus newStatus, string note)
    {
        Status = newStatus;
        _statusHistory.Add(OrderStatusHistory.Create(Id, newStatus, note));
    }

    private void EnsureStatus(OrderStatus expected)
    {
        if (Status != expected)
            throw new DomainException($"Order must be in '{expected}' status to perform this action. Current: '{Status}'.");
    }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(i => i.LineTotal);
        GrandTotal = SubTotal + ShippingCharge + VatAmount - DiscountAmount;
        if (GrandTotal < 0) GrandTotal = 0;
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
}
