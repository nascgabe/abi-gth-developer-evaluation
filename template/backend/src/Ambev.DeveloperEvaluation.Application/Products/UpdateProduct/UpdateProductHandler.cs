using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    /// <summary>
    /// Handler for updating product information
    /// </summary>
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult?>
    {
        private readonly IProductRepository _productRepository;


        /// <summary>
        /// Initializes a new instance of UpdateProductHandler
        /// </summary>
        /// <param name="productRepository">The product repository</param>
        public UpdateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Handles the update product command
        /// </summary>
        /// <param name="request">The update product command</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the product update operation</returns>
        public async Task<UpdateProductResult?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct == null)
                return null;

            existingProduct.Title = request.Title ?? existingProduct.Title;
            existingProduct.Description = request.Description ?? existingProduct.Description;
            existingProduct.Category = request.Category ?? existingProduct.Category;
            existingProduct.Image = request.Image ?? existingProduct.Image;
            existingProduct.Price = request.Price ?? existingProduct.Price;
            existingProduct.Stock = request.Stock ?? existingProduct.Stock;

            var updated = await _productRepository.UpdateAsync(existingProduct, cancellationToken);
            if (!updated)
                return null;

            return new UpdateProductResult
            {
                Id = existingProduct.Id,
                Title = existingProduct.Title,
                Description = existingProduct.Description,
                Category = existingProduct.Category,
                Image = existingProduct.Image,
                Price = existingProduct.Price,
                Stock = existingProduct.Stock
            };
        }
    }
}