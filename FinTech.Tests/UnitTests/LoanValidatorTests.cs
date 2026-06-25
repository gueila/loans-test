using FinTech.API.DTOs.Requests;
using FinTech.API.Enums;
using FinTech.API.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace FinTech.Tests.UnitTests;

public class LoanValidatorTests
{
    private readonly SimulateLoanValidator _simulateValidator = new();
    private readonly CreateLoanValidator _createValidator = new();

    [Theory]
    [InlineData(499)]
    [InlineData(50001)]
    public void SimulateValidator_ShouldFail_WhenAmountOutOfRange(decimal amount)
    {
        var request = new SimulateLoanRequest
        {
            Amount = amount,
            Term = 12,
            LoanType = LoanType.Fixed
        };

        var result = _simulateValidator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData(500)]
    [InlineData(25000)]
    [InlineData(50000)]
    public void SimulateValidator_ShouldPass_WhenAmountInRange(decimal amount)
    {
        var request = new SimulateLoanRequest
        {
            Amount = amount,
            Term = 12,
            LoanType = LoanType.Fixed
        };

        var result = _simulateValidator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(61)]
    public void SimulateValidator_ShouldFail_WhenTermOutOfRange(int term)
    {
        var request = new SimulateLoanRequest
        {
            Amount = 5000,
            Term = term,
            LoanType = LoanType.Fixed
        };

        var result = _simulateValidator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Term);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(36)]
    [InlineData(60)]
    public void SimulateValidator_ShouldPass_WhenTermInRange(int term)
    {
        var request = new SimulateLoanRequest
        {
            Amount = 5000,
            Term = term,
            LoanType = LoanType.Fixed
        };

        var result = _simulateValidator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Term);
    }

    [Fact]
    public void CreateValidator_ShouldFail_WhenUserIdIsEmpty()
    {
        var request = new CreateLoanRequest
        {
            UserId = Guid.Empty,
            Amount = 5000,
            Term = 12,
            LoanType = LoanType.Fixed
        };

        var result = _createValidator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
