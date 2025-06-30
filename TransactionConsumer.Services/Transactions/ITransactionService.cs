using TransactionConsumer.Data.Dtos;

namespace TransactionConsumer.Services.Transactions;

public interface ITransactionService
{
    Task<CreateTransactionResponse> CreateTransactionAsync(CreateTransactionRequest request, CancellationToken cancellationToken);
    Task<GetTransactionResponse?> GetTransactionAsync(Guid id, CancellationToken cancellationToken);
}