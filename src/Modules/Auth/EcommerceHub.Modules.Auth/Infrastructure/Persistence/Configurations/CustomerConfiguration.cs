using EcommerceHub.Modules.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Auth.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(x => x.IsEmailVerified).HasColumnName("is_email_verified").HasDefaultValue(false);
        builder.Property(x => x.EmailVerificationToken).HasColumnName("email_verification_token");
        builder.Property(x => x.EmailVerificationExpiry).HasColumnName("email_verification_expiry");
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.BlockReason).HasColumnName("block_reason").HasMaxLength(500);
        builder.Property(x => x.LastLoginAt).HasColumnName("last_login_at");
        builder.Property(x => x.PasswordResetToken).HasColumnName("password_reset_token");
        builder.Property(x => x.PasswordResetTokenExpiry).HasColumnName("password_reset_token_expiry");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
        builder.Property(x => x.DeletedBy).HasColumnName("deleted_by").HasMaxLength(256);

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ix_customers_email");
        builder.HasIndex(x => x.Phone).IsUnique().HasDatabaseName("ix_customers_phone");
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.RefreshTokens)
            .WithOne()
            .HasForeignKey("customer_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
