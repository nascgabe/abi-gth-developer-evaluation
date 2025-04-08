using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class GetSalesRequestValidator : AbstractValidator<GetSalesRequest>
{
    public GetSalesRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sale ID is required.")
            .When(x => x.Id.HasValue);

        RuleFor(x => x.Client)
            .MaximumLength(100).WithMessage("Client name must be at most 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Client));

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.")
            .When(x => x.ProductId.HasValue);
    }
}