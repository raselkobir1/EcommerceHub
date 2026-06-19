using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Cart.Domain.Entities;

public sealed class Cart : AuditableEntity
{
    public Guid? CustomerId { get; private set; }
    public string? SessionId { get; private set; }
    public string? CouponCode { get; private set; }
    public decimal CouponDiscount { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { }

    public static Cart CreateForCustomer(Guid customerId) =>
        new() { CustomerId = customerId };

    public static Cart CreateForGuest(string sessionId) =>
        new() { SessionId = sessionId, ExpiresAt = DateTime.UtcNow.AddDays(7) };

    public void AddItem(Guid productId, Guid variantId, string productName, string sku,
        string? imageUrl, decimal unitPrice, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.VariantId == variantId);
        if (existing is not null)
            existing.UpdateQuantity(existing.Quantity + quantity);
        else
            _items.Add(CartItem.Create(Id, productId, variantId, productName, sku, imageUrl, unitPrice, quantity));
    }

    public void UpdateItemQuantity(Guid variantId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.VariantId == variantId)
            ?? throw new NotFoundException("Cart item", variantId);

        if (quantity <= 0)
            _items.Remove(item);
        else
            item.UpdateQuantity(quantity);
    }

    public void RemoveItem(Guid variantId)
    {
        var item = _items.FirstOrDefault(i => i.VariantId == variantId);
        if (item is not null) _items.Remove(item);
    }

    public void Clear() => _items.Clear();

    public void ApplyCoupon(string couponCode, decimal discount)
    {
        CouponCode = couponCode;
        CouponDiscount = discount;
    }

    public void RemoveCoupon()
    {
        CouponCode = null;
        CouponDiscount = 0;
    }

    public void MergeWith(Cart guestCart)
    {
        foreach (var guestItem in guestCart.Items)
            AddItem(guestItem.ProductId, guestItem.VariantId, guestItem.ProductName,
                guestItem.Sku, guestItem.ImageUrl, guestItem.UnitPrice, guestItem.Quantity);
    }

    public decimal SubTotal => _items.Sum(i => i.LineTotal);
    public int TotalItems => _items.Sum(i => i.Quantity);
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
}
