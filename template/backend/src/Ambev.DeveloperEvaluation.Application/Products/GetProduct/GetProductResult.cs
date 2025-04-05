namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    /// <summary>
    /// Result for retrieving a product by its ID
    /// </summary>
    public class GetProductResult
    {
        /// <summary>
        /// The unique identifier of the product
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The product's title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The product's price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The product's description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The product's category
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// The available stock of the product
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// The URL of the product image
        /// </summary>
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// The product's rating information
        /// </summary>
        public Rating Rating { get; set; } = new Rating();

        /// <summary>
        /// The creation date of the product
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The last update date of the product (nullable)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents rating information for a product
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// The average rating of the product
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// The total number of ratings
        /// </summary>
        public int Count { get; set; }
    }
}