using System.Text;

namespace TransactionConsumer.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var request = context.Request;
        var bodyAsText = string.Empty;
        if (request.ContentLength > 0 && request.Body.CanRead)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            bodyAsText = await reader.ReadToEndAsync(context.RequestAborted);
            request.Body.Position = 0;
        }
        _logger.LogInformation("HTTP Request: {method} {path} {query} Body: {body}",
            request.Method, request.Path, request.QueryString, bodyAsText);
        await _next(context);
    }
} 