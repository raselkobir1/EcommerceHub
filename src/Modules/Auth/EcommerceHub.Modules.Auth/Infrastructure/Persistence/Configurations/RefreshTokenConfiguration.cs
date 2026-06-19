using EcommerceHub.Modules.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceHub.Modules.Auth.Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(x => x.Token).HasColumnName("token").HasMaxLength(500).IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(x => x.IsRevoked).HasColumnName("is_revoked").HasDefaultValue(false);
        builder.Property(x => x.RevokedReason).HasColumnName("revoked_reason").HasMaxLength(200);
        builder.Property(x => x.ReplacedByToken).HasColumnName("replaced_by_token").HasMaxLength(500);
        builder.Property(x => x.CreatedByIp).HasColumnName("created_by_ip").HasMaxLength(45);
        builder.Property(x => x.RevokedByIp).HasColumnName("revoked_by_ip").HasMaxLength(45);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.Token).HasDatabaseName("ix_refresh_tokens_token");
    }
}
