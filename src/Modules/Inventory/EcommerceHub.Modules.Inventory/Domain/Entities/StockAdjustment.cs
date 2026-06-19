using EcommerceHub.Modules.Inventory.Domain.Enums;
using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Inventory.Domain.Entities;

public sealed class StockAdjustment : AuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid VariantId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string Sku { get; private set; } = default!;
    public int QuantityChange { get; private set; }
    public int OldStock { get; private set; }
    public int NewStock { get; private set; }
    public AdjustmentReason Reason { get; private set; }
    public string? Notes { get; private set; }
    public string AdjustedByUserId { get; private set; } = default!;

    private StockAdjustment() { }

    public static StockAdjustment Create(
        Guid productId, Guid variantId, string productName, string sku,
        int quantityChange, int oldStock, AdjustmentReason reason,
        string adjustedByUserId, string? notes = null)
    {
        var newStock = oldStock + quantityChange;
        if (newStock < 0)
            throw new DomainException($"Adjustment would result in negative stock ({newStock}).");

        return new StockAdjustment
        {
            ProductId = productId,
            VariantId = variantId,
            ProductName = productName,
            Sku = sku,
            QuantityChange = quantityChange,
            OldStock = oldStock,
            NewStock = newStock,
            Reason = reason,
            AdjustedByUserId = adjustedByUserId,
            Notes = notes
        };
    }
}
