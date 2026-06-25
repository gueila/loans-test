using FinTech.API.Models;
using FinTech.API.Utils;

namespace FinTech.API.Strategies;

public class FixedInstallmentStrategy : ILoanCalculationStrategy
{
    public decimal CalculateMonthlyPayment(decimal amount, int term, decimal tea)
    {
        var tem = FinancialCalculator.CalculateTEM(tea);
        return Math.Round(FinancialCalculator.CalculateFixedInstallment(amount, term, tem), 2);
    }

    public List<PaymentSchedule> GenerateSchedule(Guid loanId, decimal amount, int term, decimal tea, DateTime startDate)
    {
        var tem = FinancialCalculator.CalculateTEM(tea);
        var monthlyPayment = CalculateMonthlyPayment(amount, term, tea);
        var schedules = new List<PaymentSchedule>();
        var balance = amount;

        for (int i = 1; i <= term; i++)
        {
            var interest = Math.Round(balance * tem, 2);
            var principal = Math.Round(monthlyPayment - interest, 2);
            balance = Math.Round(balance - principal, 2);

            schedules.Add(new PaymentSchedule
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                PaymentNumber = i,
                DueDate = FinancialCalculator.CalculateDueDate(startDate, i),
                TotalPayment = monthlyPayment,
                Principal = principal,
                Interest = interest,
                RemainingBalance = balance < 0 ? 0 : balance,
                Status = Models.PaymentScheduleStatus.Pending
            });
        }

        return schedules;
    }
}
