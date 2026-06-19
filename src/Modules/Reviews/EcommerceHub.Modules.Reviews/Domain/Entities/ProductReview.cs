using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Reviews.Domain.Entities;

public sealed class ProductReview : AuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public int Rating { get; private set; }
    public string? Title { get; private set; }
    public string? Body { get; private set; }
    public bool IsApproved { get; private set; }
    public bool IsVerifiedPurchase { get; private set; } = true;

    private ProductReview() { }

    public static ProductReview Create(Guid productId, Guid customerId, Guid orderId,
        int rating, string? title, string? body)
    {
        if (rating is < 1 or > 5)
            throw new DomainException("Rating must be between 1 and 5.");
        if (body is not null && body.Length > 2000)
            throw new DomainException("Review body must not exceed 2000 characters.");

        return new ProductReview
        {
            ProductId = productId, CustomerId = customerId, OrderId = orderId,
            Rating = rating, Title = title?.Trim(), Body = body?.Trim()
        };
    }

    public void Approve() => IsApproved = true;
    public void Reject() => IsApproved = false;
}
