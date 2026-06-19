namespace EcommerceHub.Modules.Reviews.Application.DTOs;

public sealed record ProductReviewDto(
    Guid Id,
    Guid ProductId,
    Guid CustomerId,
    int Rating,
    string? Title,
    string? Body,
    bool IsApproved,
    bool IsVerifiedPurchase,
    DateTime CreatedAt);
