using FinTech.API.Enums;

namespace FinTech.API.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public Guid? LoanId { get; set; }
    public Guid UserId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Loan? Loan { get; set; }
    public User User { get; set; } = null!;
}
