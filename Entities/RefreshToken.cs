namespace BackendTZ.Entities;

/// <summary>
/// Represents a refresh token entity used for authentication and authorization purposes.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the user associated with this refresh token.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Gets or sets the user associated with this refresh token.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hashed value of the refresh token.
    /// </summary>
    public required string RefreshTokenHash { get; set; }
    /// <summary>
    /// Gets or sets the date and time when the refresh token was revoked, if applicable.
    /// </summary>
    public DateTime? RevokedAt { get; set; }
    /// <summary>
    /// Gets or sets the date and time when the refresh token expires.
    /// </summary>
    public required DateTime RefreshTokenExpiry { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the refresh token that replaced this token, if applicable.
    /// </summary>
    public Guid? ReplacedByTokenId { get; set; }
}