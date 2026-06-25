using FinTech.API.Enums;

namespace FinTech.API.DTOs.Responses;

public class LoanResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public decimal InterestRate { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? MonthlyPayment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
