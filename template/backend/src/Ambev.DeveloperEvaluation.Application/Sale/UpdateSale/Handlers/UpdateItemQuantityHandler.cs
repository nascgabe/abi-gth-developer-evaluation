using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Handlers;

/// <summary>
/// Handler for processing UpdateItemQuantityCommand requests.
/// </summary>
public class UpdateItemQuantityHandler : IRequestHandler<UpdateItemQuantityCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;

    public UpdateItemQuantityHandler(ISaleRepository saleRepository, IProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var sale = await GetValidatedSaleAsync(request.SaleId, cancellationToken);
        if (sale == null)
            return false;

        var saleItem = GetValidatedSaleItem(sale, request.ItemId);
        if (saleItem == null)
            return false;

        if (request.Quantity == 0)
        {
            return await RemoveSaleItemAsync(sale, saleItem, cancellationToken);
        }

        if (!IsValidQuantity(request.Quantity))
            return false;

        await UpdateSaleItemQuantityAsync(sale, saleItem, request.Quantity, cancellationToken);
        return true;
    }

    private async Task<Domain.Entities.Sale?> GetValidatedSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        return sale != null && !sale.IsCancelled ? sale : null;
    }

    private Domain.Entities.SaleItem? GetValidatedSaleItem(Domain.Entities.Sale sale, Guid itemId)
    {
        return sale.Items.FirstOrDefault(item => item.Id == itemId);
    }

    private async Task<bool> RemoveSaleItemAsync(Domain.Entities.Sale sale, Domain.Entities.SaleItem saleItem, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(saleItem.ProductId, cancellationToken);
        if (product != null)
        {
            product.Stock += saleItem.Quantity;
            await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);
        }

        sale.Items.Remove(saleItem);
        var itemRemoved = await _saleRepository.DeleteSaleItemAsync(saleItem.Id, cancellationToken);

        if (!itemRemoved)
            return false;

        RecalculateSaleTotal(sale);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return true;
    }

    private bool IsValidQuantity(int quantity)
    {
        return quantity > 0 && quantity <= 20;
    }

    private async Task UpdateSaleItemQuantityAsync(Domain.Entities.Sale sale, Domain.Entities.SaleItem saleItem, int newQuantity, CancellationToken cancellationToken)
    {
        var quantityDifference = saleItem.Quantity - newQuantity;

        if (quantityDifference > 0)
        {
            var product = await _productRepository.GetByIdAsync(saleItem.ProductId, cancellationToken);
            if (product != null)
            {
                product.Stock += quantityDifference;
                await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);
            }
        }

        UpdateSaleItem(saleItem, newQuantity);
        RecalculateSaleTotal(sale);
        await _saleRepository.UpdateAsync(sale, cancellationToken);
    }

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

    private void RecalculateSaleTotal(Domain.Entities.Sale sale)
    {
        sale.TotalValue = sale.Items.Sum(item => item.TotalValue);
    }
}