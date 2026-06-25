using FinTech.API.DTOs.Requests;
using FluentValidation;

namespace FinTech.API.Validators;

public class CreateLoanValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El usuario es requerido");

        RuleFor(x => x.Amount)
            .InclusiveBetween(500, 50000)
            .WithMessage("El monto debe estar entre $500 y $50,000");

        RuleFor(x => x.Term)
            .InclusiveBetween(6, 60)
            .WithMessage("El plazo debe estar entre 6 y 60 meses");
    }
}
