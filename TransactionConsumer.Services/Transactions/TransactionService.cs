using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TransactionConsumer.Data;
using TransactionConsumer.Data.Dtos;
using TransactionConsumer.Data.Exceptions;
using TransactionConsumer.Data.Models;
using TransactionConsumer.Data.Settings;
using TransactionConsumer.Services.Common;

namespace TransactionConsumer.Services.Transactions;

public class TransactionService : ITransactionService
{
    private readonly TransactionDbContext _context;
    private readonly TransactionSettings _settings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TransactionService(TransactionDbContext context, 
        IOptions<TransactionSettings> settings,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _settings = settings.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<CreateTransactionResponse> CreateTransactionAsync(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var existingTransaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (existingTransaction != null)
        {
            return new CreateTransactionResponse
            {
                InsertDateTime = existingTransaction.InsertDateTime
            };
        }

        ValidateTransaction(request);

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Блокируем таблицу на обновление
            var lockSql = $"SELECT * FROM \"{nameof(_context.Transactions)}\" FOR UPDATE";
            await _context.Database.ExecuteSqlRawAsync(lockSql, cancellationToken);

            var count = await _context.Transactions.CountAsync(cancellationToken);

            if (count >= _settings.MaxTransactions)
            {
                throw new InvalidOperationException($"Transaction limit exceeded. Max: {_settings.MaxTransactions}");
            }

            var newTransaction = new Transaction
            {
                Id = request.Id,
                TransactionDate = request.TransactionDate,
                Amount = request.Amount,
                InsertDateTime = _dateTimeProvider.GetNowUtc()
            };

            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CreateTransactionResponse
            {
                InsertDateTime = newTransaction.InsertDateTime
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<GetTransactionResponse?> GetTransactionAsync(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (transaction == null)
            throw new TransactionNotFoundException($"Transaction with Id {id} not found");

        return new GetTransactionResponse
        {
            Id = transaction.Id,
            TransactionDate = transaction.TransactionDate,
            Amount = transaction.Amount
        };
    }

    private void ValidateTransaction(CreateTransactionRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentException("The transaction amount must be positive");
        }

        if (request.TransactionDate > _dateTimeProvider.GetNowUtc())
        {
            throw new ArgumentException("The transaction date cannot be in the future");
        }
    }
}