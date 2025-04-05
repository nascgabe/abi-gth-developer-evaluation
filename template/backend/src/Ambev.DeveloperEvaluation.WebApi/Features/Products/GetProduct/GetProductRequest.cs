namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct
{
    public class GetProductRequest
    {
        /// <summary>
        /// The unique identifier of the product to retrieve
        /// </summary>
        public Guid Id { get; set; }
    }
}