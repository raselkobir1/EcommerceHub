using EcommerceHub.Modules.Orders.Domain.Entities;
using EcommerceHub.Modules.Orders.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Orders.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.OrderNumber).HasColumnName("order_number").HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id");
        builder.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerEmail).HasColumnName("customer_email").HasMaxLength(256).IsRequired();
        builder.Property(x => x.CustomerPhone).HasColumnName("customer_phone").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
        builder.Property(x => x.PaymentStatus).HasColumnName("payment_status").HasConversion<string>();
        builder.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasMaxLength(50);
        builder.Property(x => x.GatewayTransactionId).HasColumnName("gateway_transaction_id").HasMaxLength(200);
        builder.Property(x => x.CourierTrackingNumber).HasColumnName("courier_tracking_number").HasMaxLength(100);
        builder.Property(x => x.DeliveryZoneId).HasColumnName("delivery_zone_id");
        builder.Property(x => x.ShippingMethod).HasColumnName("shipping_method").HasMaxLength(100);
        builder.Property(x => x.ShippingCharge).HasColumnName("shipping_charge").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(x => x.SubTotal).HasColumnName("sub_total").HasColumnType("numeric(18,2)");
        builder.Property(x => x.DiscountAmount).HasColumnName("discount_amount").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(x => x.VatAmount).HasColumnName("vat_amount").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(x => x.GrandTotal).HasColumnName("grand_total").HasColumnType("numeric(18,2)");
        builder.Property(x => x.CouponCode).HasColumnName("coupon_code").HasMaxLength(50);
        builder.Property(x => x.InternalNotes).HasColumnName("internal_notes");
        builder.Property(x => x.IsGuest).HasColumnName("is_guest").HasDefaultValue(false);
        builder.Property(x => x.EstimatedDeliveryDate).HasColumnName("estimated_delivery_date");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        // Owned type: ShippingAddress (stored in same table)
        builder.OwnsOne(x => x.ShippingAddress, sa =>
        {
            sa.Property(a => a.Division).HasColumnName("shipping_division").HasMaxLength(100);
            sa.Property(a => a.District).HasColumnName("shipping_district").HasMaxLength(100);
            sa.Property(a => a.AreaThana).HasColumnName("shipping_area_thana").HasMaxLength(100);
            sa.Property(a => a.StreetAddress).HasColumnName("shipping_street_address").HasMaxLength(500);
            sa.Property(a => a.ApartmentFloor).HasColumnName("shipping_apartment_floor").HasMaxLength(100);
        });

        builder.HasIndex(x => x.OrderNumber).IsUnique().HasDatabaseName("ix_orders_order_number");
        builder.HasIndex(x => x.CustomerId).HasDatabaseName("ix_orders_customer_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_orders_status");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_orders_created_at");

        builder.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.StatusHistory).WithOne().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.OrderId).HasColumnName("order_id");
        builder.Property(x => x.ProductId).HasColumnName("product_id");
        builder.Property(x => x.VariantId).HasColumnName("variant_id");
        builder.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(250);
        builder.Property(x => x.Sku).HasColumnName("sku").HasMaxLength(100);
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
        builder.Property(x => x.UnitPrice).HasColumnName("unit_price").HasColumnType("numeric(18,2)");
        builder.Property(x => x.Quantity).HasColumnName("quantity");
        builder.Ignore(x => x.LineTotal);
    }
}

internal sealed class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("order_status_histories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.OrderId).HasColumnName("order_id");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.OccurredAt).HasColumnName("occurred_at");
    }
}
