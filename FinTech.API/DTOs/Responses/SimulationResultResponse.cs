using FinTech.API.Enums;

namespace FinTech.API.DTOs.Responses;

public class SimulationResultResponse
{
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public decimal InterestRate { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public decimal MonthlyPayment { get; set; }
    public decimal TEM { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalPayment { get; set; }
    public List<PaymentScheduleResponse> Schedule { get; set; } = new();
}
