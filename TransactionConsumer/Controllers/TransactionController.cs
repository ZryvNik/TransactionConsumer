using Microsoft.AspNetCore.Mvc;
using TransactionConsumer.Data.Dtos;
using TransactionConsumer.Services.Transactions;

namespace TransactionConsumer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateTransactionResponse>> CreateTransaction([FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var response = await _transactionService.CreateTransactionAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetTransactionResponse>> GetTransaction([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.GetTransactionAsync(id, cancellationToken);
        return Ok(transaction);
    }
}