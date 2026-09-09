using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTZ.Data.Configurations;

public class RoomServiceConfiguration : IEntityTypeConfiguration<RoomService>
{
    public void Configure(EntityTypeBuilder<RoomService> builder)
    {
        builder.ToTable("room_services");

        builder.Property(rs => rs.RoomId).HasColumnName("room_id");
        builder.Property(rs => rs.ServiceId).HasColumnName("service_id");

        builder.HasKey(rs => new { rs.RoomId, rs.ServiceId });

        builder.HasOne(rs => rs.Room)
               .WithMany(r => r.RoomServices)
               .HasForeignKey(rs => rs.RoomId);

        builder.HasOne(rs => rs.Service)
               .WithMany(s => s.RoomServices)
               .HasForeignKey(rs => rs.ServiceId);
    }
}