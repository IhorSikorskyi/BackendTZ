using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTZ.Data.Configurations;

/// <summary>
/// Represents the configuration for the Booking entity in the database.
/// </summary>
public class BookingConfiguration : BaseEntityConfiguration<Booking>
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        base.Configure(builder);
        builder.ToTable("bookings");

        builder.Property(b => b.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(b => b.RoomId)
            .HasColumnName("room_id")
            .IsRequired();

        builder.Property(b => b.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(b => b.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(b => b.TotalCost)
            .HasColumnName("total_cost")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime });
    }
}