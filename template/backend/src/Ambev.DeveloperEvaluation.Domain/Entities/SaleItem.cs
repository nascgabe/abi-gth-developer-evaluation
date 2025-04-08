using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents an item within a sale
    /// </summary>
    public class SaleItem : BaseEntity
    {
        /// <summary>
        /// The unique identifier of the item
        /// </summary>

        public Guid SaleId { get; set; }

        /// <summary>
        /// The unique identifier of the product
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// The name of the product
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// The quantity of the product in the sale
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The unit price of the product
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// The discount applied to the product
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// The total value of the item after applying the discount
        /// </summary>
        public decimal TotalValue { get; set; }
    }
}