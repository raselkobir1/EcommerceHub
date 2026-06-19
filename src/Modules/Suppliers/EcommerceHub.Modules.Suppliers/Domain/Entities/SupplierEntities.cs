using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Suppliers.Domain.Entities;

public sealed class PurchaseOrderItem : BaseEntity
{
    public Guid PurchaseOrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid VariantId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string Sku { get; private set; } = default!;
    public int OrderedQuantity { get; private set; }
    public int ReceivedQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal LineTotal => UnitCost * OrderedQuantity;
    public bool IsFullyReceived => ReceivedQuantity >= OrderedQuantity;

    private PurchaseOrderItem() { }

    public static PurchaseOrderItem Create(Guid poId, Guid productId, Guid variantId,
        string productName, string sku, int quantity, decimal unitCost) =>
        new()
        {
            PurchaseOrderId = poId, ProductId = productId, VariantId = variantId,
            ProductName = productName, Sku = sku, OrderedQuantity = quantity, UnitCost = unitCost
        };

    public void ReceiveGoods(int quantity)
    {
        if (ReceivedQuantity + quantity > OrderedQuantity)
            throw new DomainException($"Received quantity exceeds ordered quantity for SKU {Sku}.");
        ReceivedQuantity += quantity;
    }
}

public sealed class SupplierPayment : BaseEntity
{
    public Guid SupplierId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaidAt { get; private set; }
    public string PaymentMethod { get; private set; } = default!;
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }

    private SupplierPayment() { }

    public static SupplierPayment Create(Guid supplierId, decimal amount,
        string paymentMethod, string? reference, string? notes) =>
        new()
        {
            SupplierId = supplierId, Amount = amount,
            PaidAt = DateTime.UtcNow, PaymentMethod = paymentMethod,
            Reference = reference, Notes = notes
        };
}
