namespace EcommerceHub.Modules.Promotions.Application.DTOs;

public sealed record CouponDto(
    Guid Id, string Code, string Type, decimal Value,
    decimal? MinimumOrderAmount, int? TotalUsageLimit, int? PerCustomerLimit,
    int UsedCount, DateTime StartDate, DateTime? ExpiryDate, bool IsActive);

public sealed record BannerDto(
    Guid Id, string Title, string ImageUrl, string? LinkUrl,
    bool IsActive, int DisplayOrder, DateTime? StartDate, DateTime? EndDate);

public sealed record ValidateCouponResult(
    bool IsValid, string? ErrorMessage, decimal DiscountAmount, bool IsFreeShipping);
