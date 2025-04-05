using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Valida os dados da entidade Product de acordo com as regras de domínio.
/// </summary>
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(product => product.Title)
            .NotEmpty().WithMessage("O título do produto é obrigatório.")
            .MinimumLength(3).WithMessage("O título deve ter ao menos 3 caracteres.")
            .MaximumLength(100).WithMessage("O título não pode ter mais que 100 caracteres.");

        RuleFor(product => product.Description)
            .NotEmpty().WithMessage("A descrição do produto é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição não pode ultrapassar 500 caracteres.");

        RuleFor(product => product.Category)
            .NotEmpty().WithMessage("A categoria do produto é obrigatória.")
            .MaximumLength(50).WithMessage("A categoria não pode ter mais que 50 caracteres.");

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");

        RuleFor(product => product.Image)
            .NotEmpty().WithMessage("A imagem do produto é obrigatória.")
            .Must(BeAValidUrl).WithMessage("A imagem deve ser uma URL válida.");

        RuleFor(product => product.Rating)
            .NotNull().WithMessage("As informações de avaliação são obrigatórias.");

        RuleFor(product => product.Rating.Rate)
            .InclusiveBetween(0, 5).WithMessage("A nota da avaliação deve estar entre 0 e 5.");

        RuleFor(product => product.Rating.Count)
            .GreaterThanOrEqualTo(0).WithMessage("A contagem de avaliações não pode ser negativa.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}