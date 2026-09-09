using BackendTZ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTZ.Data.Configurations;

/// <summary>
/// Represents the base configuration for entities that inherit from BaseEntity in the database.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    /// <summary>
    /// Configures the entity of type T in the database.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity.</param>
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
    }
}