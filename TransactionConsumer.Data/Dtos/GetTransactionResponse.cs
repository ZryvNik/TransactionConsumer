namespace TransactionConsumer.Data.Dtos;

public record GetTransactionResponse
{
    public required Guid Id { get; init; }
    public required DateTime TransactionDate { get; init; }
    public required decimal Amount { get; init; }
} 