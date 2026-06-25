using FinTech.API.Enums;

namespace FinTech.API.DTOs.Requests;

public class SimulateLoanRequest
{
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public LoanType LoanType { get; set; }
}
