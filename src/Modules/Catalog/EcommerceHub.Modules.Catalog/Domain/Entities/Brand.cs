using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Catalog.Domain.Entities;

public sealed class Brand : SoftDeletableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Brand() { }

    public static Brand Create(string name, string slug, string? logoUrl = null) =>
        new() { Name = name, Slug = slug.ToLowerInvariant(), LogoUrl = logoUrl };

    public void Update(string name, string slug, string? logoUrl, bool isActive)
    {
        Name = name;
        Slug = slug.ToLowerInvariant();
        LogoUrl = logoUrl;
        IsActive = isActive;
    }
}
