using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, otherwise null</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves all sales in the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all sales</returns>
    public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves last sale in the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The last sale if found, otherwise null</returns>
    public async Task<Sale?> GetLastSaleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .OrderByDescending(s => s.SaleDate)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing sale in the database
    /// </summary>
    /// <param name="sale">The sale to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale if successful, otherwise null</returns>
    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing SaleItem in the database
    /// </summary>
    /// <param name="saleItem">The SaleItem to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the update was successful, otherwise false</returns>
    public async Task UpdateSaleItemAsync(SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        _context.SaleItems.Update(saleItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale was successfully deleted, otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sale == null) return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Deletes a sale item by its unique identifier.
    /// </summary>
    /// <param name="itemId">The unique identifier of the sale item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale item was successfully deleted, otherwise false.</returns>
    public async Task<bool> DeleteSaleItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var saleItem = await _context.SaleItems.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (saleItem == null) return false;

        _context.SaleItems.Remove(saleItem);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}