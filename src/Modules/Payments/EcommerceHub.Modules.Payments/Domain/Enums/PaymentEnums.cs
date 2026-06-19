namespace EcommerceHub.Modules.Payments.Domain.Enums;

public enum PaymentMethod
{
    Cod = 1,        // Cash on Delivery
    BKash = 2,
    Nagad = 3,
    SslCommerz = 4,
    ShurjoPay = 5,
    Card = 6
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5,
    PartiallyRefunded = 6,
    Cancelled = 7
}

public enum PaymentGateway
{
    Internal = 1,
    BKash = 2,
    Nagad = 3,
    SslCommerz = 4,
    ShurjoPay = 5
}
