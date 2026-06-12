using Microsoft.AspNetCore.Mvc;
using UnitConverter.Api.Exceptions;

namespace UnitConverter.Api.Middleware;

/// <summary>
/// Catches the conversion-specific exceptions thrown by the service layer
/// and turns them into a 400 ProblemDetails response, so controllers don't
/// need a try/catch in every action. Anything else falls through to the
/// default developer exception page / generic 500 handling.
/// </summary>
public sealed class ConversionExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ConversionExceptionMiddleware> _logger;

    public ConversionExceptionMiddleware(RequestDelegate next, ILogger<ConversionExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ConversionException ex)
        {
            _logger.LogWarning(ex, "Rejected conversion request: {Message}", ex.Message);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid conversion request",
                Detail = ex.Message
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
