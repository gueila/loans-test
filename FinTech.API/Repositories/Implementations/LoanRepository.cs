using FinTech.API.Data;
using FinTech.API.Enums;
using FinTech.API.Models;
using FinTech.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTech.API.Repositories.Implementations;

public class LoanRepository : ILoanRepository
{
    private readonly ApplicationDbContext _context;

    public LoanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetByIdAsync(Guid id)
    {
        return await _context.Loans
            .Include(x => x.PaymentSchedules)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Loan>> GetAllAsync(Guid? userId = null)
    {
        var query = _context.Loans
            .Include(x => x.User)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(Guid userId)
    {
        return await _context.Loans
            .Where(x => x.UserId == userId && x.Status == LoanStatus.Active)
            .ToListAsync();
    }

    public async Task<int> GetActiveLoanCountByUserAsync(Guid userId)
    {
        return await _context.Loans
            .CountAsync(x => x.UserId == userId && x.Status == LoanStatus.Active);
    }

    public async Task<Loan> CreateAsync(Loan loan)
    {
        loan.Id = Guid.NewGuid();
        loan.CreatedAt = DateTime.UtcNow;
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan> UpdateAsync(Loan loan)
    {
        loan.UpdatedAt = DateTime.UtcNow;
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
        return loan;
    }
}
