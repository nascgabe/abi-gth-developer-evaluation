using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Handlers;

/// <summary>
/// Handler for processing CancelSaleCommand requests.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CancelSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="productRepository">The product repository</param>
    /// <param name="logger">Logger for logging events</param>
    public CancelSaleHandler(ISaleRepository saleRepository, IProductRepository productRepository, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request
    /// </summary>
    /// <param name="request">The CancelSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was successfully cancelled</returns>
    /// <exception cref="DomainException">Thrown when validation fails or sale is not found</exception>
    public async Task<bool> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await GetValidatedSaleAsync(request.SaleId, cancellationToken);

        CancelSale(sale);

        await RestoreProductStockAsync(sale.Items, cancellationToken);

        await SaveCancelledSaleAsync(sale, cancellationToken);

        _logger.LogInformation($"SaleCancelled: Sale with ID {sale.Id} was successfully cancelled.");

        return true;
    }

    /// <summary>
    /// Validates the sale and retrieves it by SaleId.
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The validated sale entity</returns>
    /// <exception cref="DomainException">Thrown if the sale is not found or already cancelled</exception>
    private async Task<Domain.Entities.Sale> GetValidatedSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        if (sale == null)
        {
            throw new DomainException($"Sale with ID {saleId} not found.");
        }

        if (sale.IsCancelled)
        {
            throw new DomainException($"Sale with ID {saleId} is already cancelled.");
        }

        return sale;
    }

    /// <summary>
    /// Cancels the sale by marking it as cancelled.
    /// </summary>
    /// <param name="sale">The sale entity to cancel</param>
    private void CancelSale(Domain.Entities.Sale sale)
    {
        sale.IsCancelled = true;
    }

    /// <summary>
    /// Restores the stock of products sold in the cancelled sale.
    /// </summary>
    /// <param name="items">The list of sale items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="DomainException">Thrown if any product is not found or stock restoration fails</exception>
    private async Task RestoreProductStockAsync(IEnumerable<Domain.Entities.SaleItem> items, CancellationToken cancellationToken)
    {
        foreach (var item in items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);

            if (product != null)
            {
                product.Stock += item.Quantity;

                var updated = await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);
                if (!updated)
                {
                    throw new DomainException($"Failed to update stock for product '{product.Title}'.");
                }
            }
            else
            {
                throw new DomainException($"Product with ID {item.ProductId} not found during stock restoration.");
            }
        }
    }

    /// <summary>
    /// Saves the cancelled sale in the repository.
    /// </summary>
    /// <param name="sale">The sale entity to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task SaveCancelledSaleAsync(Domain.Entities.Sale sale, CancellationToken cancellationToken)
    {
        await _saleRepository.UpdateAsync(sale, cancellationToken);
    }
}