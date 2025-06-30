namespace TransactionConsumer.Data.Models;

public record Transaction : BaseEntity
{ 
    public required Guid Id { get; init; } 
    public required DateTime TransactionDate { get; init; } 
    public required decimal Amount { get; init; } 
}

