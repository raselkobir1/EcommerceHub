using EcommerceHub.Modules.Payments.Domain.Entities;
using EcommerceHub.Modules.Payments.Domain.Enums;
using EcommerceHub.Shared.Kernel.Exceptions;
using FluentAssertions;
using Xunit;

namespace EcommerceHub.UnitTests.Domain.Payments;

public sealed class PaymentTests
{
    private static Payment CreatePendingPayment(decimal amount = 1500m) =>
        Payment.Create(Guid.NewGuid(), "ORD-20240101-ABCD1234",
            Guid.NewGuid(), amount, PaymentMethod.BKash, "192.168.1.1",
            "01712345678", "customer@test.com");

    [Fact]
    public void Create_ShouldSetPendingStatus()
    {
        var payment = CreatePendingPayment();

        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.Method.Should().Be(PaymentMethod.BKash);
        payment.Currency.Should().Be("BDT");
        payment.Amount.Should().Be(1500m);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldThrowDomainException()
    {
        var act = () => CreatePendingPayment(0m);
        act.Should().Throw<DomainException>().WithMessage("*positive*");
    }

    [Fact]
    public void MarkSuccess_ShouldSetCompletedStatus()
    {
        var payment = CreatePendingPayment();
        payment.MarkSuccess("TXN-BKASH-001", "REF-001");

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.GatewayTransactionId.Should().Be("TXN-BKASH-001");
        payment.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkFailed_ShouldSetFailedStatus()
    {
        var payment = CreatePendingPayment();
        payment.MarkFailed("Insufficient balance");

        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.FailureReason.Should().Be("Insufficient balance");
    }

    [Fact]
    public void ProcessRefund_OnCompletedPayment_ShouldRefund()
    {
        var payment = CreatePendingPayment(2000m);
        payment.MarkSuccess("TXN-001");

        var refund = payment.ProcessRefund(500m, "Customer request", "admin");

        refund.Amount.Should().Be(500m);
        payment.RefundedAmount.Should().Be(500m);
        payment.Status.Should().Be(PaymentStatus.PartiallyRefunded);
    }

    [Fact]
    public void ProcessRefund_FullAmount_ShouldSetRefundedStatus()
    {
        var payment = CreatePendingPayment(1000m);
        payment.MarkSuccess("TXN-001");

        payment.ProcessRefund(1000m, "Full refund", "admin");

        payment.Status.Should().Be(PaymentStatus.Refunded);
    }

    [Fact]
    public void ProcessRefund_ExceedingAmount_ShouldThrowDomainException()
    {
        var payment = CreatePendingPayment(1000m);
        payment.MarkSuccess("TXN-001");

        var act = () => payment.ProcessRefund(1500m, "Too much", "admin");

        act.Should().Throw<DomainException>().WithMessage("*exceeds*");
    }

    [Fact]
    public void ProcessRefund_OnPendingPayment_ShouldThrowDomainException()
    {
        var payment = CreatePendingPayment();

        var act = () => payment.ProcessRefund(500m, "reason", "admin");

        act.Should().Throw<DomainException>().WithMessage("*completed*");
    }

    [Fact]
    public void RaiseDomainEvent_OnCreate_ShouldContainPaymentInitiatedEvent()
    {
        var payment = CreatePendingPayment();
        payment.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "PaymentInitiatedEvent");
    }

    [Fact]
    public void RaiseDomainEvent_OnSuccess_ShouldContainPaymentCompletedEvent()
    {
        var payment = CreatePendingPayment();
        payment.MarkSuccess("TXN-001");

        payment.DomainEvents.Should().Contain(e => e.GetType().Name == "PaymentCompletedEvent");
    }
}
