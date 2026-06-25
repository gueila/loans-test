using FinTech.API.Enums;
using FinTech.API.Models;

namespace FinTech.API.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<IEnumerable<Transaction>> GetAllAsync(TransactionType? type = null, TransactionStatus? status = null);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
}
