using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTZ.Data.Configurations;

/// <summary>
/// Represents the configuration for the BookingService entity in the database.
/// </summary>
public class BookingServiceConfiguration : IEntityTypeConfiguration<BookingService>
{
    /// <summary>
    /// Configures the BookingService entity in the database.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<BookingService> builder)
    {
        builder.ToTable("booking_services");

        builder.HasKey(bs => new { bs.BookingId, bs.ServiceId });

        builder.Property(bs => bs.BookingId).HasColumnName("booking_id");
        builder.Property(bs => bs.ServiceId).HasColumnName("service_id");

        builder.HasOne(bs => bs.Booking)
               .WithMany(b => b.BookingServices)
               .HasForeignKey(bs => bs.BookingId);

        builder.HasOne(bs => bs.Service)
               .WithMany(s => s.BookingServices)
               .HasForeignKey(bs => bs.ServiceId);

        builder.Property(bs => bs.PriceAtBooking)
            .HasColumnName("price_at_booking")
            .HasPrecision(18, 2)
            .IsRequired();
    }
}