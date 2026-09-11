using System.Net;
using System.Text.Json;
using BackendTZ.Exceptions;

namespace BackendTZ.Middleware;

/// <summary>
/// Middleware for handling exceptions in the application.
/// It catches exceptions thrown during the request processing pipeline and returns
/// a standardized error response to the client.
/// </summary>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="environment">The hosting environment.</param>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    /// <summary>
    /// Invokes the middleware to handle exceptions during the request processing.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapException(exception);

        logger.LogError(
            exception,
            "Unhandled exception of type {ExceptionType} while processing request {Method} {Path}",
            exception.GetType().Name,
            context.Request.Method,
            context.Request.Path);

        var problemDetails = new
        {
            status = (int)statusCode,
            title,
            detail = environment.IsDevelopment() ? exception.Message : null,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode StatusCode, string Title) MapException(Exception exception) =>
        exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, "Incorrect input data"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "Access denied"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Operation not allowed in the current state"),
            RoomNotAvailableException => (HttpStatusCode.Conflict, "Room not available"),
            BookingConflictException => (HttpStatusCode.Conflict, "Booking conflict"),
            ValidationException => (HttpStatusCode.BadRequest, "Validation error"),
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict error"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
            InvalidCredentialsException => (HttpStatusCode.Unauthorized, "Invalid credentials"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden access"),
            SecurityException => (HttpStatusCode.Forbidden, "Security error"),
            TokenReuseDetectedException => (HttpStatusCode.Unauthorized, "Token reuse detected"),
            TokenExpiredException => (HttpStatusCode.Unauthorized, "Token expired"),
            _ => (HttpStatusCode.InternalServerError, "Internal server error")
        };
}