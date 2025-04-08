using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, otherwise null</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales in the repository
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all sales</returns>
    Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was successfully deleted, otherwise false</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale item by its unique identifier
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale item was successfully deleted, otherwise false</returns>
    Task<bool> DeleteSaleItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the last sale created in the repository
    /// </summary>
    Task<Sale?> GetLastSaleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the repository
    /// </summary>
    /// <param name="sale">The sale to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale</returns>
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing sale in the repository
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale</returns>
    Task UpdateSaleItemAsync(SaleItem saleItem, CancellationToken cancellationToken = default);
}
