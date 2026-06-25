using FinTech.API.Enums;

namespace FinTech.API.DTOs.Requests;

public class CreateLoanRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public LoanType LoanType { get; set; }
}
