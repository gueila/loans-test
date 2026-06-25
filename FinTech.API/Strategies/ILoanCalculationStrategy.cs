using FinTech.API.Models;

namespace FinTech.API.Strategies;

public interface ILoanCalculationStrategy
{
    List<PaymentSchedule> GenerateSchedule(Guid loanId, decimal amount, int term, decimal tea, DateTime startDate);
    decimal CalculateMonthlyPayment(decimal amount, int term, decimal tea);
}
