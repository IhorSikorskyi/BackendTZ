namespace BackendTZ.Entities;

/// <summary>
/// Represents a user entity in the database.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public string? Number { get; set; }
    /// <summary>
    /// Gets or sets the password hash of the user.
    /// </summary>
    public required string PasswordHash { get; set; }
    /// <summary>
    /// Gets or sets the role of the user.
    /// </summary>
    public Role Role { get; set; } = Role.User;

    /// <summary>
    /// Gets or sets the collection of bookings associated with the user.
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    /// <summary>
    /// Gets or sets the collection of refresh tokens associated with the user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

/// <summary>
/// Represents the role of a user in the system.
/// </summary>
public enum Role
{
    /// <summary>
    /// Represents an administrator role with elevated privileges.
    /// </summary>
    Admin,
    /// <summary>
    /// Represents a regular user role with standard privileges.
    /// </summary>
    User
}