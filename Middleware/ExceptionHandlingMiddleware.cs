using System.Net;
using System.Text.Json;
using BackendTZ.Exceptions;

namespace BackendTZ.Middleware;

/// <summary>
/// Централізована обробка необроблених винятків для всього API.
/// Перехоплює виключення з усього конвеєра запитів і повертає
/// уніфіковану відповідь клієнту, приховуючи внутрішні деталі реалізації.
/// </summary>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
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
            "Необроблений виняток типу {ExceptionType} під час обробки запиту {Method} {Path}",
            exception.GetType().Name,
            context.Request.Method,
            context.Request.Path);

        var problemDetails = new
        {
            status = (int)statusCode,
            title,
            // Деталі реального повідомлення показуємо лише в Development,
            // щоб не розкривати внутрішню структуру системи клієнту в проді
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