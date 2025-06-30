using System.Net;

namespace TransactionConsumer.Data.Dtos;

public record ProblemDetails
{
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public HttpStatusCode Status { get; init; }
    public string Detail { get; init; } = string.Empty;
    public string Instance { get; init; } = string.Empty;
} 