using backend.DTOs.Produto;
using FluentValidation;

namespace backend.Validators;

public class ProdutoValidator : AbstractValidator<CreateProdutoDto>
{
    public ProdutoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .WithMessage("O nome do produto é obrigatório.");

        RuleFor(p => p.Marca)
            .NotEmpty()
            .WithMessage("A marca do produto é obrigatória.");

        RuleFor(p => p.NutriScore)
            .Must(score => string.IsNullOrEmpty(score) ||
                           new[] { "A", "B", "C", "D", "E" }
                               .Contains(score.ToUpper()))
            .WithMessage("O NutriScore deve ser A, B, C, D ou E.");
    }
}