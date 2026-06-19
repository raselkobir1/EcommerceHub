namespace EcommerceHub.Modules.Orders.Domain.Enums;

public enum OrderStatus
{
    PendingPayment = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    ReturnRequested = 7,
    Returned = 8
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3,
    Refunded = 4,
    CodPending = 5
}
