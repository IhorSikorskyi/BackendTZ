namespace BackendTZ.Entities;

/// <summary>
/// Represents a user entity in the database.
/// </summary>
public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Number { get; set; }
    public required string PasswordHash { get; set; }
    public Role Role { get; set; } = Role.User;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public enum Role
{
    Admin,
    User
}