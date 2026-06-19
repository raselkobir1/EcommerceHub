using EcommerceHub.Modules.Orders.Domain.Entities;
using EcommerceHub.Modules.Orders.Domain.Enums;
using EcommerceHub.Modules.Orders.Domain.ValueObjects;
using EcommerceHub.Shared.Kernel.Exceptions;
using FluentAssertions;
using Xunit;

namespace EcommerceHub.UnitTests.Domain.Orders;

public sealed class OrderTests
{
    private static ShippingAddress DefaultAddress() =>
        ShippingAddress.Create("Dhaka", "Dhaka", "Gulshan", "House 10, Road 5", null);

    private static Order CreateConfirmedOrder()
    {
        var order = Order.Create(Guid.NewGuid(), "Test Customer", "test@example.com",
            "01712345678", DefaultAddress(), "COD");
        order.AddItem(Guid.NewGuid(), Guid.NewGuid(), "Test Product", "SKU001", null, 500m, 2);
        order.ConfirmPayment("TXN-001");
        return order;
    }

    [Fact]
    public void Create_ShouldGenerateOrderNumber_WithCorrectFormat()
    {
        var order = Order.Create(null, "Jane Doe", "jane@test.com",
            "01912345678", DefaultAddress(), "BKASH", isGuest: true);

        order.OrderNumber.Should().StartWith("ORD-");
        order.Status.Should().Be(OrderStatus.PendingPayment);
        order.IsGuest.Should().BeTrue();
    }

    [Fact]
    public void AddItem_ShouldRecalculateTotals()
    {
        var order = Order.Create(Guid.NewGuid(), "John Doe", "john@test.com",
            "01512345678", DefaultAddress(), "COD");

        order.AddItem(Guid.NewGuid(), Guid.NewGuid(), "Product A", "SKUA", null, 1000m, 3);

        order.SubTotal.Should().Be(3000m);
        order.GrandTotal.Should().Be(3000m);
    }

    [Fact]
    public void AddItem_WithSameVariant_ShouldMergeQuantity()
    {
        var order = Order.Create(Guid.NewGuid(), "John Doe", "john@test.com",
            "01512345678", DefaultAddress(), "COD");
        var variantId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        order.AddItem(productId, variantId, "Product A", "SKUA", null, 500m, 1);
        order.AddItem(productId, variantId, "Product A", "SKUA", null, 500m, 2);

        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(3);
        order.SubTotal.Should().Be(1500m);
    }

    [Fact]
    public void ConfirmPayment_ShouldChangeStatusToConfirmed()
    {
        var order = Order.Create(Guid.NewGuid(), "John Doe", "john@test.com",
            "01512345678", DefaultAddress(), "BKASH");
        order.AddItem(Guid.NewGuid(), Guid.NewGuid(), "P", "S", null, 100m, 1);

        order.ConfirmPayment("TXN-123");

        order.Status.Should().Be(OrderStatus.Confirmed);
        order.PaymentStatus.Should().Be(PaymentStatus.Paid);
        order.GatewayTransactionId.Should().Be("TXN-123");
    }

    [Fact]
    public void ConfirmPayment_WhenAlreadyPaid_ShouldThrowDomainException()
    {
        var order = CreateConfirmedOrder();

        var act = () => order.ConfirmPayment("TXN-002");

        act.Should().Throw<DomainException>()
            .WithMessage("*already paid*");
    }

    [Fact]
    public void MarkAsShipped_WhenNotProcessing_ShouldThrowDomainException()
    {
        var order = CreateConfirmedOrder();

        var act = () => order.MarkAsShipped("TRACK-001", "admin");

        act.Should().Throw<DomainException>()
            .WithMessage("*Processing*");
    }

    [Fact]
    public void FullOrderLifecycle_ShouldTransitionThroughAllStates()
    {
        var order = Order.Create(Guid.NewGuid(), "John Doe", "john@test.com",
            "01512345678", DefaultAddress(), "COD");
        order.AddItem(Guid.NewGuid(), Guid.NewGuid(), "Product", "SKU", null, 999m, 1);

        order.ConfirmPayment("COD-001");
        order.Status.Should().Be(OrderStatus.Confirmed);

        order.MarkAsProcessing("admin");
        order.Status.Should().Be(OrderStatus.Processing);

        order.MarkAsShipped("BD-TRACK-001", "admin");
        order.Status.Should().Be(OrderStatus.Shipped);
        order.CourierTrackingNumber.Should().Be("BD-TRACK-001");

        order.MarkAsDelivered("admin");
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void Cancel_WhenDelivered_ShouldThrowDomainException()
    {
        var order = Order.Create(Guid.NewGuid(), "John Doe", "john@test.com",
            "01512345678", DefaultAddress(), "COD");
        order.AddItem(Guid.NewGuid(), Guid.NewGuid(), "Product", "SKU", null, 100m, 1);
        order.ConfirmPayment("TXN");
        order.MarkAsProcessing("admin");
        order.MarkAsShipped("TRACK", "admin");
        order.MarkAsDelivered("admin");

        var act = () => order.Cancel("Wrong reason", "admin");

        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot cancel*");
    }

    [Fact]
    public void ApplyCoupon_ShouldReduceGrandTotal()
    {
        var order = Order.Create(Guid.NewGuid(), "John", "j@test.com",
            "01512345678", DefaultAddress(), "COD");
        order.AddItem(Guid.NewGuid(), Guid.NewGuid(), "Product", "SKU", null, 1000m, 1);

        order.ApplyCoupon("SAVE100", 100m);

        order.DiscountAmount.Should().Be(100m);
        order.GrandTotal.Should().Be(900m);
    }

    [Fact]
    public void RaiseDomainEvents_OnCreate_ShouldRaiseOrderPlacedEvent()
    {
        var order = Order.Create(Guid.NewGuid(), "John", "j@test.com",
            "01512345678", DefaultAddress(), "COD");

        order.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "OrderPlacedEvent");
    }
}
