using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTZ.Data.Configurations;

/// <summary>
/// Represents the configuration for the RefreshToken entity in the database.
/// </summary>
public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    ///<inheritdoc/>
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("refresh_tokens");

        builder.Property(rt => rt.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(rt => rt.RefreshTokenHash)
            .HasColumnName("refresh_token_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(rt => rt.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(rt => rt.RefreshTokenExpiry)
            .HasColumnName("refresh_token_expiry")
            .IsRequired();

        builder.Property(rt => rt.ReplacedByTokenId)
            .HasColumnName("replaced_by_token_id");

        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey(rt => rt.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(rt => rt.RefreshTokenHash)
            .IsUnique();

        builder.HasIndex(rt => rt.UserId);

        builder.HasIndex(rt => rt.RefreshTokenExpiry);
    }
}