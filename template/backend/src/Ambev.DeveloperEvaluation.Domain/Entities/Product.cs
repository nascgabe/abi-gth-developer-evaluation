using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a product in the system with details such as title, price, description, category, image, and rating.
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product title.
        /// The title should be a non-empty string.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product.
        /// The price must be a non-negative value.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// Provides additional details about the product.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of the product.
        /// Used to group products by type.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the available stock for the product.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Gets or sets the URL of the product image.
        /// </summary>
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rating information of the product.
        /// Includes average rate and number of reviews.
        /// </summary>
        public Rating Rating { get; set; } = new Rating();

        /// <summary>
        /// Gets the creation date of the product.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the product.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        public Product()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Performs validation of the product entity using the <see cref="ProductValidator"/>.
        /// </summary>
        /// <returns>A <see cref="ValidationResultDetail"/> containing validation results.</returns>
        public ValidationResultDetail Validate()
        {
            var validator = new ProductValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
            };
        }
    }

    /// <summary>
    /// Represents rating information for a product.
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// Gets or sets the average rate (e.g., 4.5).
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Gets or sets the total number of ratings.
        /// </summary>
        public int Count { get; set; }
    }
}