using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Handlers;

/// <summary>
/// Handler for processing UpdateItemQuantityCommand requests.
/// </summary>
public class CancelItemQuantityHandler : IRequestHandler<CancelItemQuantityCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Initializes a new instance of CancelItemQuantityHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="productRepository">The product repository</param>
    public CancelItemQuantityHandler(ISaleRepository saleRepository, IProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
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
            return await RemoveSaleItemAsync(sale, saleItem, cancellationToken);
        }

        if (!IsValidQuantity(request.Quantity))
        {
            throw new DomainException($"Invalid quantity: {request.Quantity}. Quantity must be between 1 and 20.");
        }

        await UpdateSaleItemQuantityAsync(sale, saleItem, request.Quantity, cancellationToken);
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
    /// <param name="sale">The sale containing the items</param>
    /// <param name="itemId">The unique identifier of the sale item</param>
    /// <returns>The validated sale item</returns>
    /// <exception cref="DomainException">Thrown if the item is not found in the sale</exception>
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
    /// <param name="sale">The sale containing the items</param>
    /// <param name="saleItem">The sale item to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the item was successfully removed</returns>
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
    /// <param name="sale">The sale containing the items</param>
    /// <param name="saleItem">The sale item to update</param>
    /// <param name="newQuantity">The new quantity for the sale item</param>
    /// <param name="cancellationToken">Cancellation token</param>
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
    /// <param name="saleItem">The sale item to update</param>
    /// <param name="newQuantity">The new quantity</param>
    private void UpdateSaleItem(Domain.Entities.SaleItem saleItem, int newQuantity)
    {
        saleItem.Quantity = newQuantity;

        if (newQuantity >= 4 && newQuantity < 10)
        {
            saleItem.Discount = saleItem.UnitPrice * newQuantity * 0.10m;
        }
        else if (newQuantity >= 10 && newQuantity <= 20)
        {
            saleItem.Discount = saleItem.UnitPrice * newQuantity * 0.20m;
        }
        else
        {
            saleItem.Discount = 0;
        }

        saleItem.TotalValue = (saleItem.UnitPrice * newQuantity) - saleItem.Discount;
    }

    /// <summary>
    /// Recalculates the total value of a sale based on its items.
    /// </summary>
    /// <param name="sale">The sale to recalculate</param>
    private void RecalculateSaleTotal(Domain.Entities.Sale sale)
    {
        sale.TotalValue = sale.Items.Sum(item => item.TotalValue);
    }

    /// <summary>
    /// Validates the quantity to ensure it is within the allowed range.
    /// </summary>
    /// <param name="quantity">The quantity to validate</param>
    /// <returns>True if the quantity is valid</returns>
    private bool IsValidQuantity(int quantity)
    {
        return quantity > 0 && quantity <= 20;
    }
}