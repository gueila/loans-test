using FinTech.API.DTOs.Requests;
using FinTech.API.DTOs.Responses;
using FinTech.API.Enums;

namespace FinTech.API.Services.Interfaces;

public interface ITransactionService
{
    Task<TransactionResponse> CreateAsync(CreateTransactionRequest request);
    Task<IEnumerable<TransactionResponse>> GetAllAsync(TransactionType? type = null, TransactionStatus? status = null);
    Task<TransactionResponse?> GetByIdAsync(Guid id);
}
