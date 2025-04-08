using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for the CreateSaleCommand.
/// </summary>
public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.SaleDate)
            .NotEmpty().WithMessage("Sale date is required.");

        RuleFor(x => x.Client)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(100).WithMessage("Client name must be at most 100 characters.");

        RuleFor(x => x.Branch)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(100).WithMessage("Branch name must be at most 100 characters.");

        RuleForEach(x => x.Items)
            .SetValidator(new SaleItemCommandValidator());
    }
}

public class SaleItemCommandValidator : AbstractValidator<SaleItemCommand>
{
    public SaleItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Quantity must not exceed 20 items per product.");
    }
}