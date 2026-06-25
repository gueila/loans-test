using FinTech.API.Enums;
using FinTech.API.Models;

namespace FinTech.API.Repositories.Interfaces;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id);
    Task<IEnumerable<Loan>> GetAllAsync(Guid? userId = null);
    Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(Guid userId);
    Task<int> GetActiveLoanCountByUserAsync(Guid userId);
    Task<Loan> CreateAsync(Loan loan);
    Task<Loan> UpdateAsync(Loan loan);
}
