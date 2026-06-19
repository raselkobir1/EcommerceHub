using System.Net;
using System.Text.Json;
using EcommerceHub.Shared.Kernel.Common;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.API.Middleware;

internal sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

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
        var (statusCode, message, errors) = exception switch
        {
            ValidationException vex => (HttpStatusCode.UnprocessableEntity, "Validation failed.", (IReadOnlyDictionary<string, string[]>?)vex.Errors),
            NotFoundException nex => (HttpStatusCode.NotFound, nex.Message, (IReadOnlyDictionary<string, string[]>?)null),
            DomainException dex => (HttpStatusCode.BadRequest, dex.Message, (IReadOnlyDictionary<string, string[]>?)null),
            UnauthorizedException uex => (HttpStatusCode.Unauthorized, uex.Message, (IReadOnlyDictionary<string, string[]>?)null),
            ForbiddenException fex => (HttpStatusCode.Forbidden, fex.Message, (IReadOnlyDictionary<string, string[]>?)null),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", (IReadOnlyDictionary<string, string[]>?)null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            logger.LogWarning(exception, "{ExceptionType}: {Message}", exception.GetType().Name, exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = errors is not null
            ? ApiResponse.Fail(message, errors.SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}")).ToArray())
            : ApiResponse.Fail(message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
