namespace EcommerceHub.Modules.Inventory.Application.DTOs;

public sealed record StockAdjustmentDto(
    Guid Id, Guid ProductId, Guid VariantId,
    string ProductName, string Sku,
    int QuantityChange, int OldStock, int NewStock,
    string Reason, string? Notes,
    string AdjustedByUserId, DateTime CreatedAt);

public sealed record InventoryItemDto(
    Guid ProductId,
    Guid VariantId,
    string ProductName,
    string Sku,
    int StockQuantity,
    int ReservedQuantity,
    int AvailableQuantity,
    int ReorderPoint,
    bool IsLowStock,
    DateTime LastUpdatedAt);
