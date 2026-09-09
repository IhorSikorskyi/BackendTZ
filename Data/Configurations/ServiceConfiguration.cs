using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTZ.Data.Configurations;

/// <summary>
/// Represents the configuration for the Service entity in the database.
/// </summary>
public class ServiceConfiguration : BaseEntityConfiguration<Service>
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<Service> builder)
    {
        base.Configure(builder);
        builder.ToTable("services");

        builder.HasIndex(s => s.Name).IsUnique();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(s => s.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();
    }
}