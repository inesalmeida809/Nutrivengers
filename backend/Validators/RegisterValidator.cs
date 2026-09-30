using backend.DTOs.Auth;
using FluentValidation;

namespace backend.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("O nome é obrigatório.");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("O username é obrigatório.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("A password deve ter pelo menos 6 caracteres.");
    }
}