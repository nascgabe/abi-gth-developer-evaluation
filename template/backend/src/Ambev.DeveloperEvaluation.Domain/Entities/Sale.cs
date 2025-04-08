using Ambev.DeveloperEvaluation.Domain.Common;

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
    }
}