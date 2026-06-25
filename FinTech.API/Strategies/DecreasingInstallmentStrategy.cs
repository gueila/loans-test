using FinTech.API.Models;
using FinTech.API.Utils;

namespace FinTech.API.Strategies;

public class DecreasingInstallmentStrategy : ILoanCalculationStrategy
{
    public decimal CalculateMonthlyPayment(decimal amount, int term, decimal tea)
    {
        var tem = FinancialCalculator.CalculateTEM(tea);
        var fixedPrincipal = FinancialCalculator.CalculateDecreasingPrincipal(amount, term);
        var firstInterest = amount * tem;
        return Math.Round(fixedPrincipal + firstInterest, 2);
    }

    public List<PaymentSchedule> GenerateSchedule(Guid loanId, decimal amount, int term, decimal tea, DateTime startDate)
    {
        var tem = FinancialCalculator.CalculateTEM(tea);
        var fixedPrincipal = Math.Round(FinancialCalculator.CalculateDecreasingPrincipal(amount, term), 2);
        var schedules = new List<PaymentSchedule>();
        var balance = amount;

        for (int i = 1; i <= term; i++)
        {
            var interest = Math.Round(balance * tem, 2);
            var totalPayment = Math.Round(fixedPrincipal + interest, 2);
            balance = Math.Round(balance - fixedPrincipal, 2);

            if (i == term)
            {
                totalPayment = Math.Round(fixedPrincipal + interest + balance, 2);
                balance = 0;
            }

            schedules.Add(new PaymentSchedule
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                PaymentNumber = i,
                DueDate = FinancialCalculator.CalculateDueDate(startDate, i),
                TotalPayment = totalPayment,
                Principal = fixedPrincipal,
                Interest = interest,
                RemainingBalance = balance < 0 ? 0 : balance,
                Status = Models.PaymentScheduleStatus.Pending
            });
        }

        return schedules;
    }
}
