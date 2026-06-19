using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Catalog.Domain.Entities;

public sealed class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Url { get; private set; } = default!;
    public string? AltText { get; private set; }
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }

    private ProductImage() { }

    public static ProductImage Create(Guid productId, string url, bool isPrimary, int sortOrder, string? altText = null) =>
        new() { ProductId = productId, Url = url, IsPrimary = isPrimary, SortOrder = sortOrder, AltText = altText };

    public void SetNotPrimary() => IsPrimary = false;
    public void SetPrimary() => IsPrimary = true;
    public void UpdateSortOrder(int order) => SortOrder = order;
}
