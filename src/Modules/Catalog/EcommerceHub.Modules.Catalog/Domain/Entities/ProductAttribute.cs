using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Catalog.Domain.Entities;

public sealed class ProductAttribute : BaseEntity
{
    public string Name { get; private set; } = default!;

    private readonly List<ProductAttributeValue> _values = [];
    public IReadOnlyCollection<ProductAttributeValue> Values => _values.AsReadOnly();

    private ProductAttribute() { }

    public static ProductAttribute Create(string name) => new() { Name = name };

    public void AddValue(string value)
    {
        if (!_values.Any(v => v.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
            _values.Add(ProductAttributeValue.Create(Id, value));
    }
}

public sealed class ProductAttributeValue : BaseEntity
{
    public Guid AttributeId { get; private set; }
    public string Value { get; private set; } = default!;

    private ProductAttributeValue() { }

    public static ProductAttributeValue Create(Guid attributeId, string value) =>
        new() { AttributeId = attributeId, Value = value };
}

public sealed class ProductVariantAttribute
{
    public Guid ProductVariantId { get; set; }
    public Guid AttributeId { get; set; }
    public Guid AttributeValueId { get; set; }
    public ProductAttribute? Attribute { get; set; }
    public ProductAttributeValue? AttributeValue { get; set; }
}

public sealed class ProductTag
{
    public Guid ProductId { get; set; }
    public Guid TagId { get; set; }
}

public sealed class ProductRelated
{
    public Guid ProductId { get; set; }
    public Guid RelatedProductId { get; set; }
}

public sealed class Tag : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;

    private Tag() { }
    public static Tag Create(string name, string slug) => new() { Name = name, Slug = slug.ToLowerInvariant() };
}
