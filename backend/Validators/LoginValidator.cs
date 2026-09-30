using backend.DTOs.Auth;
using FluentValidation;

namespace backend.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("O username é obrigatório.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A password é obrigatória.");
    }
}