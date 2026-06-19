using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Orders.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid VariantId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string Sku { get; private set; } = default!;
    public string? ImageUrl { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem() { }

    public static OrderItem Create(Guid orderId, Guid productId, Guid variantId,
        string productName, string sku, string? imageUrl, decimal unitPrice, int quantity) =>
        new()
        {
            OrderId = orderId,
            ProductId = productId,
            VariantId = variantId,
            ProductName = productName,
            Sku = sku,
            ImageUrl = imageUrl,
            UnitPrice = unitPrice,
            Quantity = quantity
        };

    public void IncreaseQuantity(int amount) => Quantity += amount;
}
