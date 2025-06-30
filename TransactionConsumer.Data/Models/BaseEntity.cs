namespace TransactionConsumer.Data.Models
{
    public record BaseEntity
    {
        public required DateTime InsertDateTime { get; init; }
    }
}
