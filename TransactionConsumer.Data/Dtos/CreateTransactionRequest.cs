namespace TransactionConsumer.Data.Dtos;

public record CreateTransactionRequest
{
    public required Guid Id { get; init; }
    public required DateTime TransactionDate { get; init; }
    public required decimal Amount { get; init; }
} 