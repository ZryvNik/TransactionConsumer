using System.Net;
using System.Text.Json;
using TransactionConsumer.Data.Dtos;

namespace TransactionConsumer.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Ошибка валидации", cancellationToken: context.RequestAborted);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Ошибка бизнес-логики", cancellationToken: context.RequestAborted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Внутренняя ошибка сервера");
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера", "Произошла непредвиденная ошибка", cancellationToken: context.RequestAborted);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode, string title, string? detail = null, CancellationToken cancellationToken = default)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new ProblemDetails
        {
            Type = "",
            Title = title,
            Status = statusCode,
            Detail = detail ?? ex.Message,
            Instance = context.Request.Path
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options), cancellationToken);
    }
} 