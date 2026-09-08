namespace BackendTZ.Entities;

/// <summary>
/// Represents the base entity class for all entities in the database.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}