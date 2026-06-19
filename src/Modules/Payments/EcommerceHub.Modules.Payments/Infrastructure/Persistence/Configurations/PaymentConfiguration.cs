using EcommerceHub.Modules.Payments.Domain.Entities;
using EcommerceHub.Modules.Payments.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Payments.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(x => x.OrderNumber).HasColumnName("order_number").HasMaxLength(30).IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired().HasDefaultValue("BDT");
        builder.Property(x => x.Method).HasColumnName("method").HasConversion<string>().IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().IsRequired();
        builder.Property(x => x.GatewayTransactionId).HasColumnName("gateway_transaction_id").HasMaxLength(200);
        builder.Property(x => x.GatewayReference).HasColumnName("gateway_reference").HasMaxLength(500);
        builder.Property(x => x.GatewayResponse).HasColumnName("gateway_response").HasColumnType("text");
        builder.Property(x => x.FailureReason).HasColumnName("failure_reason").HasMaxLength(1000);
        builder.Property(x => x.PaidAt).HasColumnName("paid_at");
        builder.Property(x => x.RefundedAt).HasColumnName("refunded_at");
        builder.Property(x => x.RefundedAmount).HasColumnName("refunded_amount").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(x => x.RefundReason).HasColumnName("refund_reason").HasMaxLength(500);
        builder.Property(x => x.CustomerPhone).HasColumnName("customer_phone").HasMaxLength(20);
        builder.Property(x => x.CustomerEmail).HasColumnName("customer_email").HasMaxLength(256);
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(45).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasMany(x => x.Refunds).WithOne().HasForeignKey(r => r.PaymentId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId).HasDatabaseName("ix_payments_order_id");
        builder.HasIndex(x => x.CustomerId).HasDatabaseName("ix_payments_customer_id");
        builder.HasIndex(x => x.GatewayTransactionId).HasDatabaseName("ix_payments_gateway_tx_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_payments_status");

        builder.Navigation(x => x.Refunds).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class PaymentRefundConfiguration : IEntityTypeConfiguration<PaymentRefund>
{
    public void Configure(EntityTypeBuilder<PaymentRefund> builder)
    {
        builder.ToTable("payment_refunds");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PaymentId).HasColumnName("payment_id").IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();
        builder.Property(x => x.InitiatedBy).HasColumnName("initiated_by").HasMaxLength(256).IsRequired();
        builder.Property(x => x.RefundedAt).HasColumnName("refunded_at").IsRequired();
        builder.Property(x => x.GatewayRefundId).HasColumnName("gateway_refund_id").HasMaxLength(200);
    }
}
