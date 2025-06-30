using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TransactionConsumer.Data.Dtos;
using TransactionConsumer.Data.Exceptions;
using TransactionConsumer.Data.Settings;

namespace TransactionConsumer.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly string _urlReadme;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IOptions<SpecSettings> options)
    {
        _next = next;
        _logger = logger;
        _urlReadme = options.Value.UrlReadme;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Validation error", cancellationToken: context.RequestAborted);
        }
        catch(TransactionNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, "Business logic error", cancellationToken: context.RequestAborted);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Business logic error", cancellationToken: context.RequestAborted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error");
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Internal server error", "An unexpected error occurred", context.RequestAborted);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, 
        Exception ex, 
        HttpStatusCode statusCode, 
        string title, 
        string? detail = null, 
        CancellationToken cancellationToken = default)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new ProblemDetails
        {
            Type = _urlReadme,
            Title = title,
            Status = statusCode,
            Detail = detail ?? ex.Message,
            Instance = context.Request.Path
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options), cancellationToken);
    }
}