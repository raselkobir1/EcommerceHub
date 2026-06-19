using EcommerceHub.Modules.Suppliers.Domain.Enums;
using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Suppliers.Domain.Entities;

public sealed class PurchaseOrder : AuditableEntity
{
    public string PoNumber { get; private set; } = default!;
    public Guid SupplierId { get; private set; }
    public PurchaseOrderStatus Status { get; private set; } = PurchaseOrderStatus.Draft;
    public DateTime? ExpectedDeliveryDate { get; private set; }
    public string? Notes { get; private set; }
    public decimal TotalAmount { get; private set; }

    private readonly List<PurchaseOrderItem> _items = [];
    public IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

    private readonly List<SupplierPayment> _payments = [];
    public IReadOnlyCollection<SupplierPayment> Payments => _payments.AsReadOnly();

    public bool IsReceived => Status is PurchaseOrderStatus.PartialReceived or PurchaseOrderStatus.FullyReceived;

    private PurchaseOrder() { }

    public static PurchaseOrder Create(Guid supplierId, DateTime? expectedDeliveryDate, string? notes)
    {
        var po = new PurchaseOrder
        {
            PoNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            SupplierId = supplierId,
            ExpectedDeliveryDate = expectedDeliveryDate,
            Notes = notes
        };
        return po;
    }

    public void AddItem(Guid productId, Guid variantId, string productName, string sku,
        int quantity, decimal unitCost)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Items can only be added to a Draft PO.");
        _items.Add(PurchaseOrderItem.Create(Id, productId, variantId, productName, sku, quantity, unitCost));
        RecalculateTotal();
    }

    public void Send()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Only Draft POs can be sent.");
        if (!_items.Any())
            throw new DomainException("PO must have at least one item.");
        Status = PurchaseOrderStatus.Sent;
    }

    public void ReceiveGoods(IEnumerable<(Guid ItemId, int ReceivedQuantity)> receipts)
    {
        if (Status == PurchaseOrderStatus.FullyReceived)
            throw new DomainException("PO is already fully received.");

        foreach (var (itemId, qty) in receipts)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId)
                ?? throw new NotFoundException("PO Item", itemId);
            item.ReceiveGoods(qty);
        }

        Status = _items.All(i => i.IsFullyReceived)
            ? PurchaseOrderStatus.FullyReceived
            : PurchaseOrderStatus.PartialReceived;
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.FullyReceived)
            throw new DomainException("Cannot cancel a fully received PO.");
        Status = PurchaseOrderStatus.Cancelled;
    }

    private void RecalculateTotal() =>
        TotalAmount = _items.Sum(i => i.LineTotal);
}
