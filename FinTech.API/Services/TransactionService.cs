using FinTech.API.DTOs.Requests;
using FinTech.API.DTOs.Responses;
using FinTech.API.Enums;
using FinTech.API.Models;
using FinTech.API.Repositories.Interfaces;
using FinTech.API.Services.Interfaces;

namespace FinTech.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request)
    {
        var existing = await _transactionRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey);
        if (existing is not null)
            return MapToResponse(existing);

        var transaction = new Models.Transaction
        {
            IdempotencyKey = request.IdempotencyKey,
            Type = request.Type,
            Amount = request.Amount,
            Status = TransactionStatus.Pending,
            LoanId = request.LoanId,
            UserId = request.UserId,
            Description = request.Description
        };

        transaction = await _transactionRepository.CreateAsync(transaction);

        transaction.Status = TransactionStatus.Completed;
        transaction = await _transactionRepository.UpdateAsync(transaction);

        return MapToResponse(transaction);
    }

    public async Task<IEnumerable<TransactionResponse>> GetAllAsync(TransactionType? type = null, TransactionStatus? status = null)
    {
        var transactions = await _transactionRepository.GetAllAsync(type, status);
        return transactions.Select(MapToResponse);
    }

    public async Task<TransactionResponse?> GetByIdAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        return transaction is null ? null : MapToResponse(transaction);
    }

    private static TransactionResponse MapToResponse(Models.Transaction transaction)
    {
        return new TransactionResponse
        {
            Id = transaction.Id,
            IdempotencyKey = transaction.IdempotencyKey,
            Type = transaction.Type.ToString(),
            Amount = transaction.Amount,
            Status = transaction.Status.ToString(),
            LoanId = transaction.LoanId,
            UserId = transaction.UserId,
            UserName = transaction.User?.Name ?? "",
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt
        };
    }
}
