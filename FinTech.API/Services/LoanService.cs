using FinTech.API.DTOs.Requests;
using FinTech.API.DTOs.Responses;
using FinTech.API.Enums;
using FinTech.API.Repositories.Interfaces;
using FinTech.API.Services.Interfaces;
using FinTech.API.Strategies;
using FinTech.API.Utils;

namespace FinTech.API.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly LoanCalculationFactory _calculationFactory;
    private const decimal MinInterestRate = 0.18m;
    private const decimal MaxInterestRate = 0.35m;

    public LoanService(
        ILoanRepository loanRepository,
        ITransactionRepository transactionRepository,
        LoanCalculationFactory calculationFactory)
    {
        _loanRepository = loanRepository;
        _transactionRepository = transactionRepository;
        _calculationFactory = calculationFactory;
    }

    public Task<SimulationResultResponse> SimulateAsync(SimulateLoanRequest request)
    {
        var tea = CalculateInterestRate(request.Amount);
        var strategy = _calculationFactory.GetStrategy(request.LoanType);
        var schedule = strategy.GenerateSchedule(Guid.Empty, request.Amount, request.Term, tea, DateTime.UtcNow);
        var monthlyPayment = strategy.CalculateMonthlyPayment(request.Amount, request.Term, tea);

        var response = new SimulationResultResponse
        {
            Amount = request.Amount,
            Term = request.Term,
            InterestRate = tea,
            LoanType = request.LoanType.ToString(),
            MonthlyPayment = monthlyPayment,
            TEM = FinancialCalculator.CalculateTEM(tea),
            TotalInterest = schedule.Sum(x => x.Interest),
            TotalPayment = schedule.Sum(x => x.TotalPayment),
            Schedule = schedule.Select(x => new PaymentScheduleResponse
            {
                PaymentNumber = x.PaymentNumber,
                DueDate = x.DueDate,
                TotalPayment = x.TotalPayment,
                Principal = x.Principal,
                Interest = x.Interest,
                RemainingBalance = x.RemainingBalance,
                Status = x.Status.ToString()
            }).ToList()
        };

        return Task.FromResult(response);
    }

    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request)
    {
        var activeLoansCount = await _loanRepository.GetActiveLoanCountByUserAsync(request.UserId);
        if (activeLoansCount >= 3)
            throw new InvalidOperationException("El cliente no puede tener más de 3 préstamos activos simultáneamente");

        var tea = CalculateInterestRate(request.Amount);
        var strategy = _calculationFactory.GetStrategy(request.LoanType);
        var monthlyPayment = strategy.CalculateMonthlyPayment(request.Amount, request.Term, tea);

        var loan = new Models.Loan
        {
            UserId = request.UserId,
            Amount = request.Amount,
            Term = request.Term,
            InterestRate = tea,
            LoanType = request.LoanType,
            Status = LoanStatus.Pending,
            MonthlyPayment = monthlyPayment
        };

        var paymentSchedules = strategy.GenerateSchedule(Guid.Empty, request.Amount, request.Term, tea, DateTime.UtcNow.AddMonths(1));
        foreach (var schedule in paymentSchedules)
            loan.PaymentSchedules.Add(schedule);

        loan = await _loanRepository.CreateAsync(loan);

        return MapToResponse(loan);
    }

    public async Task<IEnumerable<LoanResponse>> GetAllAsync(Guid? userId = null)
    {
        var loans = await _loanRepository.GetAllAsync(userId);
        return loans.Select(MapToResponse);
    }

    public async Task<LoanResponse?> GetByIdAsync(Guid id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        return loan is null ? null : MapToResponse(loan);
    }

    public async Task<IEnumerable<PaymentScheduleResponse>> GetScheduleAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan is null)
            throw new KeyNotFoundException("Préstamo no encontrado");

        return loan.PaymentSchedules
            .OrderBy(x => x.PaymentNumber)
            .Select(x => new PaymentScheduleResponse
            {
                PaymentNumber = x.PaymentNumber,
                DueDate = x.DueDate,
                TotalPayment = x.TotalPayment,
                Principal = x.Principal,
                Interest = x.Interest,
                RemainingBalance = x.RemainingBalance,
                Status = x.Status.ToString()
            });
    }

    public async Task<LoanResponse?> ApproveAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan is null)
            throw new KeyNotFoundException("Préstamo no encontrado");

        if (loan.Status != LoanStatus.Pending)
            throw new InvalidOperationException($"El préstamo no está en estado pendiente. Estado actual: {loan.Status}");

        loan.Status = LoanStatus.Approved;
        await _loanRepository.UpdateAsync(loan);

        var disbursementTransaction = new Models.Transaction
        {
            IdempotencyKey = $"DISBURSEMENT_{loanId}",
            Type = TransactionType.Disbursement,
            Amount = loan.Amount,
            Status = TransactionStatus.Completed,
            LoanId = loan.Id,
            UserId = loan.UserId,
            Description = $"Desembolso de préstamo #{loan.Id}"
        };
        await _transactionRepository.CreateAsync(disbursementTransaction);

        loan.Status = LoanStatus.Active;
        await _loanRepository.UpdateAsync(loan);

        return MapToResponse(loan);
    }

    public async Task<LoanResponse?> RejectAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan is null)
            throw new KeyNotFoundException("Préstamo no encontrado");

        if (loan.Status != LoanStatus.Pending)
            throw new InvalidOperationException($"El préstamo no está en estado pendiente. Estado actual: {loan.Status}");

        loan.Status = LoanStatus.Rejected;
        await _loanRepository.UpdateAsync(loan);

        return MapToResponse(loan);
    }

    private static decimal CalculateInterestRate(decimal amount)
    {
        if (amount < 10000) return 0.22m;
        if (amount < 25000) return 0.24m;
        if (amount < 40000) return 0.28m;
        return 0.32m;
    }

    private static LoanResponse MapToResponse(Models.Loan loan)
    {
        return new LoanResponse
        {
            Id = loan.Id,
            UserId = loan.UserId,
            UserName = loan.User?.Name ?? "",
            Amount = loan.Amount,
            Term = loan.Term,
            InterestRate = loan.InterestRate,
            LoanType = loan.LoanType.ToString(),
            Status = loan.Status.ToString(),
            MonthlyPayment = loan.MonthlyPayment,
            CreatedAt = loan.CreatedAt,
            UpdatedAt = loan.UpdatedAt
        };
    }
}
