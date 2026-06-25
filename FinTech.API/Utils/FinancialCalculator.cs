namespace FinTech.API.Utils;

public static class FinancialCalculator
{
    public static decimal CalculateTEM(decimal tea)
    {
        return (decimal)(Math.Pow(1 + (double)tea, 1.0 / 12.0) - 1);
    }

    public static decimal CalculateFixedInstallment(decimal amount, int term, decimal tem)
    {
        var factor = (decimal)Math.Pow(1 + (double)tem, term);
        return amount * (tem * factor) / (factor - 1);
    }

    public static decimal CalculateDecreasingPrincipal(decimal amount, int term)
    {
        return amount / term;
    }

    public static DateTime CalculateDueDate(DateTime startDate, int paymentNumber)
    {
        var dueDate = startDate.AddMonths(paymentNumber);
        var originalDay = startDate.Day;

        var daysInMonth = DateTime.DaysInMonth(dueDate.Year, dueDate.Month);
        if (originalDay > daysInMonth)
        {
            dueDate = new DateTime(dueDate.Year, dueDate.Month, daysInMonth, 0, 0, 0, DateTimeKind.Utc);
        }
        else
        {
            dueDate = new DateTime(dueDate.Year, dueDate.Month, originalDay, 0, 0, 0, DateTimeKind.Utc);
        }

        return dueDate;
    }
}
