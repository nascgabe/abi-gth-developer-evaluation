using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Handlers;

/// <summary>
/// Handler for processing CancelSaleCommand requests.
/// </summary>
public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;

    public CancelSaleCommandHandler(ISaleRepository saleRepository, IProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);

        if (sale == null || sale.IsCancelled)
        {
            return false;
        }

        sale.IsCancelled = true;

        foreach (var item in sale.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);

            if (product != null)
            {
                product.Stock += item.Quantity;
                await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);
            }
        }

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return true;
    }
}
