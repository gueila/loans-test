using FinTech.API.Data;
using FinTech.API.Enums;
using FinTech.API.Models;
using FinTech.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTech.API.Repositories.Implementations;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions
            .Include(x => x.Loan)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await _context.Transactions
            .Include(x => x.Loan)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey);
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync(TransactionType? type = null, TransactionStatus? status = null)
    {
        var query = _context.Transactions
            .Include(x => x.Loan)
            .Include(x => x.User)
            .AsQueryable();

        if (type.HasValue)
            query = query.Where(x => x.Type == type.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        transaction.CreatedAt = DateTime.UtcNow;
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }
}
