using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Catalog.Domain.Entities;

public sealed class ProductVariant : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = default!;
    public decimal? PriceOverride { get; private set; }
    public int StockQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<ProductVariantAttribute> _attributes = [];
    public IReadOnlyCollection<ProductVariantAttribute> Attributes => _attributes.AsReadOnly();

    private ProductVariant() { }

    public static ProductVariant Create(
        Guid productId, string sku, decimal? priceOverride, int stock,
        IEnumerable<(Guid AttributeId, Guid AttributeValueId)> attributes)
    {
        var variant = new ProductVariant
        {
            ProductId = productId,
            Sku = sku.ToUpperInvariant(),
            PriceOverride = priceOverride,
            StockQuantity = stock
        };
        foreach (var (attrId, valId) in attributes)
            variant._attributes.Add(new ProductVariantAttribute
            {
                ProductVariantId = variant.Id,
                AttributeId = attrId,
                AttributeValueId = valId
            });
        return variant;
    }

    public void AdjustStock(int quantity)
    {
        if (StockQuantity + quantity < 0)
            throw new DomainException($"Insufficient stock. Available: {AvailableStock}");
        StockQuantity += quantity;
    }

    public void Reserve(int quantity)
    {
        if (quantity > AvailableStock)
            throw new DomainException($"Cannot reserve {quantity} units. Available: {AvailableStock}");
        ReservedQuantity += quantity;
    }

    public void ReleaseReservation(int quantity) =>
        ReservedQuantity = Math.Max(0, ReservedQuantity - quantity);

    public void ConfirmReservation(int quantity)
    {
        StockQuantity -= quantity;
        ReservedQuantity = Math.Max(0, ReservedQuantity - quantity);
    }

    public int AvailableStock => StockQuantity - ReservedQuantity;
}
