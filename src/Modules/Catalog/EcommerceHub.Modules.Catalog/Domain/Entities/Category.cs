using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Catalog.Domain.Entities;

public sealed class Category : SoftDeletableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? ImageUrl { get; private set; }
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }

    private readonly List<Category> _children = [];
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { }

    public static Category Create(string name, string slug, Guid? parentId = null,
        string? imageUrl = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Category name is required.");
        if (string.IsNullOrWhiteSpace(slug)) throw new DomainException("Category slug is required.");

        return new Category
        {
            Name = name,
            Slug = slug.ToLowerInvariant(),
            ParentId = parentId,
            ImageUrl = imageUrl,
            SortOrder = sortOrder
        };
    }

    public void Update(string name, string slug, Guid? parentId,
        string? imageUrl, int sortOrder, bool isActive)
    {
        Name = name;
        Slug = slug.ToLowerInvariant();
        ParentId = parentId;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void SetSeo(string? metaTitle, string? metaDescription)
    {
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
