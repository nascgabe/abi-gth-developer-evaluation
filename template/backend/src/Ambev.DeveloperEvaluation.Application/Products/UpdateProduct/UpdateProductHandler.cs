using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    /// <summary>
    /// Handler for updating product information
    /// </summary>
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// Initializes a new instance of UpdateProductHandler
        /// </summary>
        /// <param name="productRepository">The product repository</param>
        public UpdateProductHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the UpdateProductCommand request
        /// </summary>
        /// <param name="request">The UpdateProduct command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated product details</returns>
        public async Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            ValidateCommand(request, cancellationToken);

            var existingProduct = await GetExistingProduct(request.Id, cancellationToken);

            await UpdateProduct(existingProduct, request, cancellationToken);

            return _mapper.Map<UpdateProductResult>(existingProduct);
        }

        /// <summary>
        /// Validates the UpdateProductCommand
        /// </summary>
        /// <param name="command">The command to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private void ValidateCommand(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateProductValidator();
            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        /// <summary>
        /// Retrieves the existing product by ID
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The existing product</returns>
        private async Task<Product> GetExistingProduct(Guid productId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
                throw new DomainException($"Product with ID {productId} not found.");

            return product;
        }

        /// <summary>
        /// Updates product fields and saves the updated product in the repository
        /// </summary>
        /// <param name="product">The existing product</param>
        /// <param name="command">The command with updated fields</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task UpdateProduct(Product product, UpdateProductCommand command, CancellationToken cancellationToken)
        {
            product.Title = command.Title ?? product.Title;
            product.Description = command.Description ?? product.Description;
            product.Category = command.Category ?? product.Category;
            product.Image = command.Image ?? product.Image;
            product.Price = command.Price ?? product.Price;
            product.Stock = command.Stock ?? product.Stock;

            var updated = await _productRepository.UpdateAsync(product, cancellationToken);
            if (!updated)
                throw new DomainException("Failed to update the product.");
        }
    }
}