using FinTech.API.Data;
using FinTech.API.Enums;
using FinTech.API.Models;
using FinTech.API.Repositories.Implementations;
using FinTech.API.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Tests.IntegrationTests;

public class TransactionDeduplicationTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"FinTechTest_{Guid.NewGuid()}")
            .Options;

        var context = new ApplicationDbContext(options);

        context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = "hash",
            MonthlyIncome = 5000,
            CreatedAt = DateTime.UtcNow
        });
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task CreateTransaction_WithSameIdempotencyKey_ShouldReturnExistingTransaction()
    {
        using var context = CreateDbContext();
        var repo = new TransactionRepository(context);
        var userId = context.Users.First().Id;

        var transaction1 = new Transaction
        {
            IdempotencyKey = "KEY_001",
            Type = TransactionType.Payment,
            Amount = 500.00m,
            Status = TransactionStatus.Completed,
            UserId = userId,
            Description = "First attempt"
        };

        var result1 = await repo.CreateAsync(transaction1);

        var existing = await repo.GetByIdempotencyKeyAsync("KEY_001");

        existing.Should().NotBeNull();
        existing!.Id.Should().Be(result1.Id);
        existing.Amount.Should().Be(500.00m);
    }

    [Fact]
    public async Task CreateTransaction_WithDifferentKeys_ShouldCreateDifferentTransactions()
    {
        using var context = CreateDbContext();
        var repo = new TransactionRepository(context);
        var userId = context.Users.First().Id;

        var tx1 = new Transaction
        {
            IdempotencyKey = "KEY_A",
            Type = TransactionType.Payment,
            Amount = 100m,
            Status = TransactionStatus.Completed,
            UserId = userId
        };

        var tx2 = new Transaction
        {
            IdempotencyKey = "KEY_B",
            Type = TransactionType.Payment,
            Amount = 200m,
            Status = TransactionStatus.Completed,
            UserId = userId
        };

        var result1 = await repo.CreateAsync(tx1);
        var result2 = await repo.CreateAsync(tx2);

        result1.Id.Should().NotBe(result2.Id);
    }
}
