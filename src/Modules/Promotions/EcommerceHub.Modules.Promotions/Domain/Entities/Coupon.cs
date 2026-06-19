using EcommerceHub.Modules.Promotions.Domain.Enums;
using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Promotions.Domain.Entities;

public sealed class Coupon : AuditableEntity
{
    public string Code { get; private set; } = default!;
    public CouponType Type { get; private set; }
    public decimal Value { get; private set; }
    public decimal? MinimumOrderAmount { get; private set; }
    public CouponScope Scope { get; private set; } = CouponScope.AllProducts;
    public int? TotalUsageLimit { get; private set; }
    public int? PerCustomerLimit { get; private set; }
    public int UsedCount { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<CouponScopeItem> _scopeItems = [];
    public IReadOnlyCollection<CouponScopeItem> ScopeItems => _scopeItems.AsReadOnly();

    private readonly List<CouponUsage> _usages = [];
    public IReadOnlyCollection<CouponUsage> Usages => _usages.AsReadOnly();

    private Coupon() { }

    public static Coupon Create(string code, CouponType type, decimal value,
        DateTime startDate, DateTime? expiryDate,
        decimal? minimumOrderAmount = null,
        int? totalUsageLimit = null, int? perCustomerLimit = null)
    {
        if (value <= 0) throw new DomainException("Coupon value must be positive.");
        if (type == CouponType.Percentage && value > 100)
            throw new DomainException("Percentage coupon cannot exceed 100%.");

        return new Coupon
        {
            Code = code.ToUpperInvariant(),
            Type = type,
            Value = value,
            StartDate = startDate,
            ExpiryDate = expiryDate,
            MinimumOrderAmount = minimumOrderAmount,
            TotalUsageLimit = totalUsageLimit,
            PerCustomerLimit = perCustomerLimit
        };
    }

    public bool IsValid(decimal orderAmount, Guid? customerId)
    {
        if (!IsActive) return false;
        if (DateTime.UtcNow < StartDate) return false;
        if (ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value) return false;
        if (MinimumOrderAmount.HasValue && orderAmount < MinimumOrderAmount.Value) return false;
        if (TotalUsageLimit.HasValue && UsedCount >= TotalUsageLimit.Value) return false;
        if (PerCustomerLimit.HasValue && customerId.HasValue)
        {
            var customerUsage = _usages.Count(u => u.CustomerId == customerId.Value);
            if (customerUsage >= PerCustomerLimit.Value) return false;
        }
        return true;
    }

    public decimal CalculateDiscount(decimal orderAmount)
    {
        return Type switch
        {
            CouponType.Percentage => Math.Round(orderAmount * Value / 100, 2),
            CouponType.FixedAmount => Math.Min(Value, orderAmount),
            CouponType.FreeShipping => 0,
            _ => 0
        };
    }

    public void RecordUsage(Guid? customerId, Guid orderId)
    {
        UsedCount++;
        _usages.Add(CouponUsage.Create(Id, customerId, orderId));
    }

    public void Update(
        string code,
        CouponType type,
        decimal value,
        DateTime startDate,
        DateTime? expiryDate,
        decimal? minimumOrderAmount,
        int? totalUsageLimit,
        int? perCustomerLimit)
    {
        if (value <= 0) throw new DomainException("Coupon value must be positive.");
        if (type == CouponType.Percentage && value > 100)
            throw new DomainException("Percentage coupon cannot exceed 100%.");

        Code = code.ToUpperInvariant();
        Type = type;
        Value = value;
        StartDate = startDate;
        ExpiryDate = expiryDate;
        MinimumOrderAmount = minimumOrderAmount;
        TotalUsageLimit = totalUsageLimit;
        PerCustomerLimit = perCustomerLimit;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void Toggle() => IsActive = !IsActive;
}
