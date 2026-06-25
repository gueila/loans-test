using FinTech.API.Enums;
using FinTech.API.Strategies;
using FinTech.API.Utils;
using FluentAssertions;

namespace FinTech.Tests.UnitTests;

public class FinancialCalculatorTests
{
    [Fact]
    public void CalculateFixedInstallment_ShouldReturnCorrectValue()
    {
        var amount = 5000m;
        var term = 12;
        var tea = 0.24m;
        var tem = FinancialCalculator.CalculateTEM(tea);
        var strategy = new FixedInstallmentStrategy();

        var monthlyPayment = strategy.CalculateMonthlyPayment(amount, term, tea);
        var expectedTem = (decimal)(Math.Pow(1.0 + 0.24, 1.0 / 12.0) - 1);
        var expectedPayment = amount * (expectedTem * (decimal)Math.Pow(1 + (double)expectedTem, term))
            / ((decimal)Math.Pow(1 + (double)expectedTem, term) - 1);

        monthlyPayment.Should().Be(Math.Round(expectedPayment, 2));
        monthlyPayment.Should().Be(467.26m);
    }

    [Fact]
    public void GenerateSchedule_ShouldReturnCorrectNumberOfPayments()
    {
        var amount = 5000m;
        var term = 12;
        var tea = 0.24m;
        var strategy = new FixedInstallmentStrategy();

        var schedule = strategy.GenerateSchedule(Guid.NewGuid(), amount, term, tea, DateTime.UtcNow);

        schedule.Should().HaveCount(12);
        schedule.Should().BeInAscendingOrder(s => s.PaymentNumber);
        schedule.Last().RemainingBalance.Should().BeLessThanOrEqualTo(1.00m);
    }

    [Fact]
    public void GenerateSchedule_ForDecreasing_ShouldHaveConstantPrincipal()
    {
        var amount = 6000m;
        var term = 6;
        var tea = 0.24m;
        var strategy = new DecreasingInstallmentStrategy();

        var schedule = strategy.GenerateSchedule(Guid.NewGuid(), amount, term, tea, DateTime.UtcNow);

        schedule.Should().HaveCount(6);
        schedule.Should().BeInAscendingOrder(s => s.PaymentNumber);
        schedule.Last().RemainingBalance.Should().Be(0);
    }

    [Fact]
    public void CalculateTEM_ShouldReturnCorrectValue()
    {
        var tea = 0.24m;
        var tem = FinancialCalculator.CalculateTEM(tea);

        var expected = (decimal)(Math.Pow(1.24, 1.0 / 12.0) - 1);
        tem.Should().Be(expected);
        tem.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CalculateDueDate_ShouldHandleMonthEndCorrectly()
    {
        var startDate = new DateTime(2025, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        var dueDate1 = FinancialCalculator.CalculateDueDate(startDate, 1);
        var dueDate2 = FinancialCalculator.CalculateDueDate(startDate, 2);

        dueDate1.Day.Should().Be(28);
        dueDate2.Day.Should().Be(31);
    }
}
