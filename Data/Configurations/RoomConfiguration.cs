using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTZ.Data.Configurations;

/// <summary>
/// Represents the configuration for the Room entity in the database.
/// </summary>
public class RoomConfiguration : BaseEntityConfiguration<Room>
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<Room> builder)
    {
        base.Configure(builder);

        builder.ToTable("rooms");

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Capacity)
            .HasColumnName("capacity")
            .IsRequired();

        builder.Property(r => r.BaseHourlyRate)
            .HasColumnName("base_hourly_rate")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.IsAvailable)
            .HasColumnName("is_available")
            .HasDefaultValue(true)
            .IsRequired();
    }
}