namespace BackendTZ.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a room is not available for booking.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class RoomNotAvailableException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when a booking conflict occurs, such as overlapping bookings for the same room.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class BookingConflictException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when a validation error occurs.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class ValidationException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when a requested resource is not found.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class NotFoundException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when a conflict occurs, such as a duplicate resource.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class ConflictException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when an unauthorized action is attempted.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class UnauthorizedException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when invalid credentials are provided.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class InvalidCredentialsException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when access to a resource is forbidden, typically due to insufficient permissions.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class ForbiddenException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when a security violation occurs.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class SecurityException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when a token reuse is detected.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class TokenReuseDetectedException(string message) : Exception(message);

/// <summary>
/// Represents an exception that is thrown when a token has expired.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public class TokenExpiredException(string message) : Exception(message);