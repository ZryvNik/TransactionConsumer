namespace TransactionConsumer.Data.Dtos;

public record CreateTransactionResponse
{
    public required DateTime InsertDateTime { get; init; }
} 