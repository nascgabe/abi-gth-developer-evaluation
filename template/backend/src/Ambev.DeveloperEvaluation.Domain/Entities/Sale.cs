using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a sale in the system
    /// </summary>
    public class Sale : BaseEntity
    {
        /// <summary>
        /// The number of the sale
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// The date when the sale was conducted
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// The client who made the purchase
        /// </summary>
        public string Client { get; set; } = string.Empty;

        /// <summary>
        /// The branch where the sale was conducted
        /// </summary>
        public string Branch { get; set; } = string.Empty;

        /// <summary>
        /// The list of items in the sale
        /// </summary>
        public List<SaleItem> Items { get; set; } = new();

        /// <summary>
        /// Indicates whether the sale is canceled
        /// </summary>
        public bool IsCancelled { get; set; } = false;

        /// <summary>
        /// The total value of the sale
        /// </summary>
        public decimal TotalValue { get; set; }

        /// <summary>
        /// Validates the sale entity using the SaleValidator.
        /// </summary>
        /// <returns>A validation result indicating if the entity is valid.</returns>
        public ValidationResult Validate()
        {
            var validator = new SaleValidator();
            var validationResult = validator.Validate(this);

            return new ValidationResult(validationResult.Errors
                .Select(error => error.ErrorMessage).ToList());
        }

        /// <summary>
        /// Calculates the total value of the sale based on its items.
        /// </summary>
        public void CalculateTotalValue()
        {
            TotalValue = Items.Sum(item => (item.UnitPrice * item.Quantity) - item.Discount);
        }
    }

    /// <summary>
    /// Represents the result of a validation operation.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public List<string> Errors { get; }

        public ValidationResult(List<string> errors)
        {
            Errors = errors;
        }
    }
}