using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;
using Ambev.DeveloperEvaluation.Application.Utilities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Handlers;

/// <summary>
/// Handler for processing CancelItemQuantityCommand requests.
/// </summary>
public class CancelItemQuantityHandler : IRequestHandler<CancelItemQuantityCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CancelItemQuantityHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CancelItemQuantityHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="productRepository">The product repository</param>
    /// <param name="logger">Logger instance for logging events</param>
    public CancelItemQuantityHandler(ISaleRepository saleRepository, IProductRepository productRepository, ILogger<CancelItemQuantityHandler> logger)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CancelItemQuantityCommand request
    /// </summary>
    /// <param name="request">The CancelItemQuantity command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the item quantity was successfully cancelled or adjusted</returns>
    public async Task<bool> Handle(CancelItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var sale = await GetValidatedSaleAsync(request.SaleId, cancellationToken);

        var saleItem = GetValidatedSaleItem(sale, request.ItemId);

        if (request.Quantity == 0)
        {
            var result = await RemoveSaleItemAsync(sale, saleItem, cancellationToken);

            if (result)
            {
                _logger.LogInformation($"ItemCancelled: Item with ID {saleItem.Id} was removed from sale {sale.Id}.");
            }

            return result;
        }

        if (!IsValidQuantity(request.Quantity))
        {
            throw new DomainException($"Invalid quantity: {request.Quantity}. Quantity must be between 1 and 20.");
        }

        await UpdateSaleItemQuantityAsync(sale, saleItem, request.Quantity, cancellationToken);

        _logger.LogInformation($"ItemQuantityCancelled: Quantity for item {saleItem.Id} in sale {sale.Id} was updated to {request.Quantity}.");

        return true;
    }

    /// <summary>
    /// Validates the sale and retrieves it by SaleId.
    /// </summary>
    private async Task<Domain.Entities.Sale> GetValidatedSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);

        if (sale is null)
        {
            throw new DomainException($"The sale with ID {saleId} was not found.");
        }

        if (sale.IsCancelled)
        {
            throw new DomainException($"The sale with ID {saleId} is already cancelled. No changes are allowed.");
        }

        return sale;
    }

    /// <summary>
    /// Validates the sale item and retrieves it from the sale.
    /// </summary>
    private Domain.Entities.SaleItem GetValidatedSaleItem(Domain.Entities.Sale sale, Guid itemId)
    {
        var saleItem = sale.Items.FirstOrDefault(item => item.Id == itemId);
        if (saleItem is null)
        {
            throw new DomainException($"The sale item with ID {itemId} was not found in the sale.");
        }

        return saleItem;
    }

    /// <summary>
    /// Removes a sale item and restores the product stock.
    /// </summary>
    private async Task<bool> RemoveSaleItemAsync(Domain.Entities.Sale sale, Domain.Entities.SaleItem saleItem, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(saleItem.ProductId, cancellationToken);
        if (product is not null)
        {
            product.Stock += saleItem.Quantity;
            await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);
        }

        sale.Items.Remove(saleItem);
        var itemRemoved = await _saleRepository.DeleteSaleItemAsync(saleItem.Id, cancellationToken);

        if (!itemRemoved)
        {
            throw new DomainException($"Failed to remove the sale item with ID {saleItem.Id}.");
        }

        RecalculateSaleTotal(sale);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return true;
    }

    /// <summary>
    /// Updates the quantity of a sale item and adjusts the stock.
    /// </summary>
    private async Task UpdateSaleItemQuantityAsync(Domain.Entities.Sale sale, Domain.Entities.SaleItem saleItem, int newQuantity, CancellationToken cancellationToken)
    {
        var quantityDifference = saleItem.Quantity - newQuantity;

        if (quantityDifference > 0)
        {
            var product = await _productRepository.GetByIdAsync(saleItem.ProductId, cancellationToken);
            if (product is not null)
            {
                product.Stock += quantityDifference;
                var updated = await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);
                if (!updated)
                {
                    throw new DomainException($"Failed to update the stock for product '{product.Title}'.");
                }
            }
        }

        UpdateSaleItem(saleItem, newQuantity);
        RecalculateSaleTotal(sale);
        await _saleRepository.UpdateAsync(sale, cancellationToken);
    }

    /// <summary>
    /// Updates the properties of a sale item based on the new quantity.
    /// </summary>
    private void UpdateSaleItem(Domain.Entities.SaleItem saleItem, int newQuantity)
    {
        PricingCalculator.ApplyOrUpdatePricing(saleItem, saleItem.UnitPrice, newQuantity, saleItem.ProductName);
    }

    /// <summary>
    /// Recalculates the total value of a sale based on its items.
    /// </summary>
    private void RecalculateSaleTotal(Domain.Entities.Sale sale)
    {
        sale.TotalValue = sale.Items.Sum(item => item.TotalValue);
    }

    /// <summary>
    /// Validates the quantity to ensure it is within the allowed range.
    /// </summary>
    private bool IsValidQuantity(int quantity)
    {
        return quantity > 0 && quantity <= 20;
    }
}