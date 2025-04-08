using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validates the Sale entity to ensure it meets the required business rules.
/// </summary>
public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleDate)
            .NotEmpty()
            .Must(saleDate => saleDate <= DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond))
            .WithMessage("Sale date cannot be in the future.");


        RuleFor(sale => sale.Client)
            .NotEmpty()
            .WithMessage("Client is required.");

        RuleFor(sale => sale.Branch)
            .NotEmpty()
            .WithMessage("Branch is required.");

        RuleFor(sale => sale.Items)
            .NotEmpty()
            .WithMessage("Sale must contain at least one item.");

        RuleFor(sale => sale.TotalValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total value cannot be negative.");
    }
}