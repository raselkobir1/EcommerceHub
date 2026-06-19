using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Cart.Domain.Entities;

public sealed class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid VariantId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string Sku { get; private set; } = default!;
    public string? ImageUrl { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public bool IsSavedForLater { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    private CartItem() { }

    public static CartItem Create(Guid cartId, Guid productId, Guid variantId,
        string productName, string sku, string? imageUrl, decimal unitPrice, int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        return new CartItem
        {
            CartId = cartId, ProductId = productId, VariantId = variantId,
            ProductName = productName, Sku = sku, ImageUrl = imageUrl,
            UnitPrice = unitPrice, Quantity = quantity
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        Quantity = quantity;
    }

    public void UpdatePrice(decimal price) => UnitPrice = price;
    public void SaveForLater() => IsSavedForLater = true;
    public void MoveToCart() => IsSavedForLater = false;
}
