using FinTech.API.Enums;

namespace FinTech.API.Strategies;

public class LoanCalculationFactory
{
    private readonly Dictionary<LoanType, ILoanCalculationStrategy> _strategies;

    public LoanCalculationFactory()
    {
        _strategies = new Dictionary<LoanType, ILoanCalculationStrategy>
        {
            { LoanType.Fixed, new FixedInstallmentStrategy() },
            { LoanType.Decreasing, new DecreasingInstallmentStrategy() }
        };
    }

    public ILoanCalculationStrategy GetStrategy(LoanType loanType)
    {
        if (_strategies.TryGetValue(loanType, out var strategy))
            return strategy;

        throw new ArgumentException($"No strategy found for loan type: {loanType}");
    }
}
