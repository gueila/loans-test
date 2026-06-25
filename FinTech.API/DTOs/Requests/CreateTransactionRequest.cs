using FinTech.API.Enums;

namespace FinTech.API.DTOs.Requests;

public class CreateTransactionRequest
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public Guid? LoanId { get; set; }
    public Guid UserId { get; set; }
    public string? Description { get; set; }
}
