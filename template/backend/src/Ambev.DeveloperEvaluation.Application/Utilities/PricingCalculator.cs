namespace Ambev.DeveloperEvaluation.Application.Utilities
{
    /// <summary>
    /// Provides methods for applying and updating pricing logic for sale items.
    /// </summary>
    public static class PricingCalculator
    {
        /// <summary>
        /// Applies or updates pricing for a sale item based on the unit price, quantity, and product name.
        /// </summary>
        /// <param name="item">The sale item to which pricing will be applied.</param>
        /// <param name="unitPrice">The unit price of the product.</param>
        /// <param name="quantity">The quantity of the product.</param>
        /// <param name="productName">The name of the product (optional).</param>
        public static void ApplyOrUpdatePricing(Domain.Entities.SaleItem item, decimal unitPrice, int quantity, string productName = "Unknown")
        {
            item.UnitPrice = unitPrice;
            item.Quantity = quantity;
            item.ProductName = productName;

            if (quantity >= 4 && quantity < 10)
            {
                item.Discount = unitPrice * quantity * 0.10m;
            }
            else if (quantity >= 10 && quantity <= 20)
            {
                item.Discount = unitPrice * quantity * 0.20m;
            }
            else
            {
                item.Discount = 0;
            }

            item.TotalValue = (unitPrice * quantity) - item.Discount;
        }
    }
}