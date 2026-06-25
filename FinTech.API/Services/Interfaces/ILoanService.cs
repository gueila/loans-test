using FinTech.API.DTOs.Requests;
using FinTech.API.DTOs.Responses;

namespace FinTech.API.Services.Interfaces;

public interface ILoanService
{
    Task<SimulationResultResponse> SimulateAsync(SimulateLoanRequest request);
    Task<LoanResponse> CreateAsync(CreateLoanRequest request);
    Task<IEnumerable<LoanResponse>> GetAllAsync(Guid? userId = null);
    Task<LoanResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentScheduleResponse>> GetScheduleAsync(Guid loanId);
    Task<LoanResponse?> ApproveAsync(Guid loanId);
    Task<LoanResponse?> RejectAsync(Guid loanId);
}
