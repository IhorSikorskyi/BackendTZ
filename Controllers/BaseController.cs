using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendTZ.Controllers;

/// <summary>
/// Base controller class that provides common functionality for all API controllers.
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Gets the current user's ID from the claims in the JWT token.
    /// </summary>
    /// <returns>The user's ID if available; otherwise, null.</returns>
    protected Guid? GetCurrentUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(value, out var id) ? id : null;
    }
}