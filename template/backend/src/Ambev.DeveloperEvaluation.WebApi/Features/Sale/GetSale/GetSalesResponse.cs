namespace Ambev.DeveloperEvaluation.WebApi.Features.Sale.GetSale
{
    public class GetSalesResponse
    {
        /// <summary>
        /// API response model for sale operations
        /// </summary>
        public class SaleResponse
        {
            public Guid Id { get; set; }
            public string SaleNumber { get; set; } = string.Empty;
            public DateTime SaleDate { get; set; }
            public string Client { get; set; } = string.Empty;
            public string Branch { get; set; } = string.Empty;
            public decimal TotalValue { get; set; }
            public List<SaleItemResponse> Items { get; set; } = new();
        }

        /// <summary>
        /// Represents an item in a sale response
        /// </summary>
        public class SaleItemResponse
        {
            public Guid ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Discount { get; set; }
            public decimal TotalValue { get; set; }
        }
    }
}