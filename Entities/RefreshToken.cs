namespace BackendTZ.Entities;

/// <summary>
/// Represents a refresh token entity used for authentication and authorization purposes.
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public required string RefreshTokenHash { get; set; }
    public DateTime? RevokedAt { get; set; }
    public required DateTime RefreshTokenExpiry { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
}