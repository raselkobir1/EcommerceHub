using EcommerceHub.Modules.Suppliers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Suppliers.Infrastructure.Persistence.Configurations;

internal sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CompanyName).HasColumnName("company_name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.ContactPerson).HasColumnName("contact_person").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
        builder.Property(x => x.BankAccountDetails).HasColumnName("bank_account_details").HasMaxLength(500);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
        builder.Property(x => x.DeletedBy).HasColumnName("deleted_by").HasMaxLength(256);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false).IsRequired();

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ix_suppliers_email");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_suppliers_is_active");

        builder.HasMany(x => x.PurchaseOrders)
            .WithOne()
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.PurchaseOrders).UsePropertyAccessMode(PropertyAccessMode.Field);

        // SupplierPayments are owned at the Supplier level; configured in SupplierPaymentConfiguration.
    }
}

internal sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PoNumber).HasColumnName("po_number").HasMaxLength(30).IsRequired();
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().IsRequired();
        builder.Property(x => x.ExpectedDeliveryDate).HasColumnName("expected_delivery_date");
        builder.Property(x => x.Notes).HasColumnName("notes").HasColumnType("text");
        builder.Property(x => x.TotalAmount).HasColumnName("total_amount").HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);

        builder.HasIndex(x => x.PoNumber).IsUnique().HasDatabaseName("ix_purchase_orders_po_number");
        builder.HasIndex(x => x.SupplierId).HasDatabaseName("ix_purchase_orders_supplier_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_purchase_orders_status");

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("purchase_order_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
        builder.Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(x => x.VariantId).HasColumnName("variant_id").IsRequired();
        builder.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(300).IsRequired();
        builder.Property(x => x.Sku).HasColumnName("sku").HasMaxLength(100).IsRequired();
        builder.Property(x => x.OrderedQuantity).HasColumnName("ordered_quantity").IsRequired();
        builder.Property(x => x.ReceivedQuantity).HasColumnName("received_quantity").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.UnitCost).HasColumnName("unit_cost").HasColumnType("numeric(18,2)").IsRequired();

        builder.Ignore(x => x.LineTotal);
        builder.Ignore(x => x.IsFullyReceived);

        builder.HasIndex(x => x.PurchaseOrderId).HasDatabaseName("ix_purchase_order_items_purchase_order_id");
        builder.HasIndex(x => x.ProductId).HasDatabaseName("ix_purchase_order_items_product_id");
        builder.HasIndex(x => x.VariantId).HasDatabaseName("ix_purchase_order_items_variant_id");
    }
}

internal sealed class SupplierPaymentConfiguration : IEntityTypeConfiguration<SupplierPayment>
{
    public void Configure(EntityTypeBuilder<SupplierPayment> builder)
    {
        builder.ToTable("supplier_payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.PaidAt).HasColumnName("paid_at").IsRequired();
        builder.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Reference).HasColumnName("payment_reference").HasMaxLength(200);
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);

        builder.HasIndex(x => x.SupplierId).HasDatabaseName("ix_supplier_payments_supplier_id");
        builder.HasIndex(x => x.PaidAt).HasDatabaseName("ix_supplier_payments_paid_at");
    }
}
