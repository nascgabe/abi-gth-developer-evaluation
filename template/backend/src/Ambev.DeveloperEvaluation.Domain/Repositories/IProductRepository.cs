using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Product entity operations
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Creates a new product in the repository
    /// </summary>
    /// <param name="product">The product to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created product</returns>
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a product by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product if found, otherwise null</returns>
    
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the product was successfully deleted, otherwise false</returns>
    /// 
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product in the repository.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the product was successfully updated, otherwise false.</returns>
    Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default);


    /// <summary>
    /// Updates the stock of a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="quantity">The new stock value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the stock was successfully updated, otherwise false.</returns>
    Task<bool> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}