using FinTech.API.Data.Configurations;
using FinTech.API.Enums;
using FinTech.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTech.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<PaymentSchedule> PaymentSchedules => Set<PaymentSchedule>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new LoanConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var userId1 = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        var userId2 = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
        var loanId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = userId1,
                Name = "Juan Pérez",
                Email = "juan@email.com",
                PasswordHash = "$2a$11$placeholder",
                MonthlyIncome = 5000.00m,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = userId2,
                Name = "María García",
                Email = "maria@email.com",
                PasswordHash = "$2a$11$placeholder",
                MonthlyIncome = 8000.00m,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Loan>().HasData(
            new Loan
            {
                Id = loanId,
                UserId = userId1,
                Amount = 5000.00m,
                Term = 12,
                InterestRate = 0.24m,
                LoanType = LoanType.Fixed,
                Status = LoanStatus.Active,
                MonthlyPayment = 467.26m,
                CreatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<PaymentSchedule>().HasData(
            new PaymentSchedule
            {
                Id = Guid.Parse("d4e5f6a7-b8c9-0123-def4-567890123456"),
                LoanId = loanId,
                PaymentNumber = 1,
                DueDate = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 376.83m,
                Interest = 90.43m,
                RemainingBalance = 4623.17m,
                Status = PaymentScheduleStatus.Paid
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("e5f6a7b8-c9d0-1234-ef56-789012345678"),
                LoanId = loanId,
                PaymentNumber = 2,
                DueDate = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 383.65m,
                Interest = 83.61m,
                RemainingBalance = 4239.52m,
                Status = PaymentScheduleStatus.Paid
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("f6a7b8c9-d0e1-2345-f678-901234567890"),
                LoanId = loanId,
                PaymentNumber = 3,
                DueDate = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 390.59m,
                Interest = 76.67m,
                RemainingBalance = 3848.93m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("a7b8c9d0-e1f2-3456-7890-123456789abc"),
                LoanId = loanId,
                PaymentNumber = 4,
                DueDate = new DateTime(2025, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 397.65m,
                Interest = 69.61m,
                RemainingBalance = 3451.28m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("b8c9d0e1-f2a3-4567-8901-234567890abc"),
                LoanId = loanId,
                PaymentNumber = 5,
                DueDate = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 404.85m,
                Interest = 62.41m,
                RemainingBalance = 3046.43m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("c9d0e1f2-a3b4-5678-9012-345678901abc"),
                LoanId = loanId,
                PaymentNumber = 6,
                DueDate = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 412.17m,
                Interest = 55.09m,
                RemainingBalance = 2634.26m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("d0e1f2a3-b4c5-6789-0123-45678901abcd"),
                LoanId = loanId,
                PaymentNumber = 7,
                DueDate = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 419.63m,
                Interest = 47.63m,
                RemainingBalance = 2214.63m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("e1f2a3b4-c5d6-7890-1234-5678901abcde"),
                LoanId = loanId,
                PaymentNumber = 8,
                DueDate = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 427.22m,
                Interest = 40.04m,
                RemainingBalance = 1787.41m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("f2a3b4c5-d6e7-8901-2345-678901abcdef"),
                LoanId = loanId,
                PaymentNumber = 9,
                DueDate = new DateTime(2025, 10, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 434.95m,
                Interest = 32.31m,
                RemainingBalance = 1352.46m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("a3b4c5d6-e7f8-9012-3456-789012345678"),
                LoanId = loanId,
                PaymentNumber = 10,
                DueDate = new DateTime(2025, 11, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 442.82m,
                Interest = 24.44m,
                RemainingBalance = 909.64m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("b4c5d6e7-f8a9-0123-4567-890123456789"),
                LoanId = loanId,
                PaymentNumber = 11,
                DueDate = new DateTime(2025, 12, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 450.83m,
                Interest = 16.43m,
                RemainingBalance = 458.81m,
                Status = PaymentScheduleStatus.Pending
            },
            new PaymentSchedule
            {
                Id = Guid.Parse("c5d6e7f8-a9b0-1234-5678-901234567890"),
                LoanId = loanId,
                PaymentNumber = 12,
                DueDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalPayment = 467.26m,
                Principal = 458.81m,
                Interest = 8.30m,
                RemainingBalance = 0.00m,
                Status = PaymentScheduleStatus.Pending
            }
        );
    }
}
