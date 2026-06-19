using EcommerceHub.Modules.Promotions.Domain.Enums;
using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Promotions.Domain.Entities;

public sealed class CouponScopeItem : BaseEntity
{
    public Guid CouponId { get; set; }
    public Guid ReferenceId { get; set; }
    public CouponScope Scope { get; set; }
    private CouponScopeItem() { }
}

public sealed class CouponUsage : BaseEntity
{
    public Guid CouponId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public DateTime UsedAt { get; private set; } = DateTime.UtcNow;
    private CouponUsage() { }
    public static CouponUsage Create(Guid couponId, Guid? customerId, Guid orderId) =>
        new() { CouponId = couponId, CustomerId = customerId, OrderId = orderId };
}

public sealed class FlashSale : AuditableEntity
{
    public Guid ProductId { get; private set; }
    public decimal FlashPrice { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    private FlashSale() { }

    public static FlashSale Create(Guid productId, decimal flashPrice, DateTime startAt, DateTime endAt) =>
        new() { ProductId = productId, FlashPrice = flashPrice, StartAt = startAt, EndAt = endAt };

    public bool IsCurrentlyActive =>
        IsActive && DateTime.UtcNow >= StartAt && DateTime.UtcNow <= EndAt;

    public void Deactivate() => IsActive = false;
}

public sealed class Banner : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string? Subtitle { get; private set; }
    public string ImageUrl { get; private set; } = default!;
    public string? LinkUrl { get; private set; }
    public int SortOrder { get; private set; }
    public DateTime? ActiveFrom { get; private set; }
    public DateTime? ActiveTo { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Banner() { }

    public static Banner Create(string title, string imageUrl, string? subtitle = null,
        string? linkUrl = null, int sortOrder = 0, DateTime? activeFrom = null, DateTime? activeTo = null) =>
        new()
        {
            Title = title, ImageUrl = imageUrl, Subtitle = subtitle,
            LinkUrl = linkUrl, SortOrder = sortOrder,
            ActiveFrom = activeFrom, ActiveTo = activeTo
        };

    public bool IsCurrentlyActive =>
        IsActive &&
        (!ActiveFrom.HasValue || DateTime.UtcNow >= ActiveFrom.Value) &&
        (!ActiveTo.HasValue || DateTime.UtcNow <= ActiveTo.Value);

    public void Update(string title, string imageUrl, string? subtitle,
        string? linkUrl, int sortOrder, DateTime? activeFrom, DateTime? activeTo)
    {
        Title = title;
        ImageUrl = imageUrl;
        Subtitle = subtitle;
        LinkUrl = linkUrl;
        SortOrder = sortOrder;
        ActiveFrom = activeFrom;
        ActiveTo = activeTo;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
