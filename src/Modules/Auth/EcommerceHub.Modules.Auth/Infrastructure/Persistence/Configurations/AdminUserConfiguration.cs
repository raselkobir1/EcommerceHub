using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Auth.Infrastructure.Persistence.Configurations;

internal sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("admin_users");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").HasConversion<string>().IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.TwoFactorEnabled).HasColumnName("two_factor_enabled").HasDefaultValue(false);
        builder.Property(x => x.TwoFactorSecret).HasColumnName("two_factor_secret");
        builder.Property(x => x.LastLoginAt).HasColumnName("last_login_at");
        builder.Property(x => x.LastLoginIp).HasColumnName("last_login_ip").HasMaxLength(45);
        builder.Property(x => x.FailedLoginCount).HasColumnName("failed_login_count").HasDefaultValue(0);
        builder.Property(x => x.LockoutEndAt).HasColumnName("lockout_end_at");
        builder.Property(x => x.PasswordResetToken).HasColumnName("password_reset_token");
        builder.Property(x => x.PasswordResetTokenExpiry).HasColumnName("password_reset_token_expiry");

        // Audit
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(256);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(256);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
        builder.Property(x => x.DeletedBy).HasColumnName("deleted_by").HasMaxLength(256);

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ix_admin_users_email");
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.RefreshTokens)
            .WithOne()
            .HasForeignKey("admin_user_id")
            .OnDelete(DeleteBehavior.Cascade);

        // Seed: Super Admin (password: Admin@12345)
        builder.HasData(new
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            FullName = "Super Admin",
            Email = "superadmin@ecommercehub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@12345", workFactor: 12),
            Role = AdminRole.SuperAdmin,
            IsActive = true,
            TwoFactorEnabled = false,
            FailedLoginCount = 0,
            IsDeleted = false,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "system"
        });
    }
}
