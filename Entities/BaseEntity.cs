namespace BackendTZ.Entities;

/// <summary>
/// Represents the base entity class for all entities in the database.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}