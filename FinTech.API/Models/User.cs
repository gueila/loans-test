namespace FinTech.API.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal MonthlyIncome { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
