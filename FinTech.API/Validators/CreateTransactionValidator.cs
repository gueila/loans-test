using FinTech.API.DTOs.Requests;
using FluentValidation;

namespace FinTech.API.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithMessage("La clave de idempotencia es requerida");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("El monto debe ser mayor a 0");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El usuario es requerido");
    }
}
