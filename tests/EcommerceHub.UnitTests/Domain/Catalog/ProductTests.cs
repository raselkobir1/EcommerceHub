using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Enums;
using EcommerceHub.Shared.Kernel.Exceptions;
using FluentAssertions;
using Xunit;

namespace EcommerceHub.UnitTests.Domain.Catalog;

public sealed class ProductTests
{
    [Fact]
    public void Create_ShouldSetInitialState()
    {
        var categoryId = Guid.NewGuid();
        var product = Product.Create("Test Shirt", "test-shirt", "TSH001",
            999m, categoryId, "A test shirt");

        product.Name.Should().Be("Test Shirt");
        product.Slug.Should().Be("test-shirt");
        product.Sku.Should().Be("TSH001");
        product.RegularPrice.Should().Be(999m);
        product.CategoryId.Should().Be(categoryId);
        product.Status.Should().Be(ProductStatus.Draft);
        product.IsVisible.Should().BeFalse();
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowDomainException()
    {
        var act = () => Product.Create("P", "p", "SKU", -1m, Guid.NewGuid());
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Publish_WithoutImages_ShouldThrowDomainException()
    {
        var product = Product.Create("Product", "product", "SKU", 100m, Guid.NewGuid());

        var act = () => product.Publish();

        act.Should().Throw<DomainException>()
            .WithMessage("*image*");
    }

    [Fact]
    public void Publish_WithImages_ShouldSetStatusToActive()
    {
        var product = Product.Create("Product", "product", "SKU", 100m, Guid.NewGuid());
        product.AddImage("https://example.com/img.jpg", true, 1);

        product.Publish();

        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void AddVariant_WithDuplicateSku_ShouldThrowDomainException()
    {
        var product = Product.Create("P", "p", "SKU", 100m, Guid.NewGuid());
        product.AddVariant("VAR-001", null, 10, []);

        var act = () => product.AddVariant("VAR-001", null, 5, []);

        act.Should().Throw<DomainException>()
            .WithMessage("*VAR-001*");
    }

    [Fact]
    public void AddRelatedProduct_MaxEight_ShouldThrowDomainException()
    {
        var product = Product.Create("P", "p", "SKU", 100m, Guid.NewGuid());
        for (int i = 0; i < 8; i++)
            product.AddRelatedProduct(Guid.NewGuid());

        var act = () => product.AddRelatedProduct(Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("*8*");
    }

    [Fact]
    public void IsOnSale_WhenSalePriceLessThanRegular_ShouldBeTrue()
    {
        var product = Product.Create("P", "p", "SKU", 1000m, Guid.NewGuid());
        product.Update("P", "p", null, null, 1000m, 800m, product.CategoryId, null, false, 0);

        product.IsOnSale.Should().BeTrue();
        product.EffectivePrice.Should().Be(800m);
    }

    [Fact]
    public void Archive_ShouldSetStatusToArchived()
    {
        var product = Product.Create("P", "p", "SKU", 100m, Guid.NewGuid());
        product.Archive();

        product.Status.Should().Be(ProductStatus.Archived);
    }

    [Fact]
    public void AddImage_PrimaryFlag_ShouldOnlyHaveOnePrimary()
    {
        var product = Product.Create("P", "p", "SKU", 100m, Guid.NewGuid());
        product.AddImage("https://example.com/a.jpg", true, 1);
        product.AddImage("https://example.com/b.jpg", true, 2);

        product.Images.Count(i => i.IsPrimary).Should().Be(1);
    }

    [Fact]
    public void RaiseDomainEvents_OnCreate_ShouldContainProductCreatedEvent()
    {
        var product = Product.Create("P", "p", "SKU", 100m, Guid.NewGuid());
        product.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "ProductCreatedEvent");
    }
}
