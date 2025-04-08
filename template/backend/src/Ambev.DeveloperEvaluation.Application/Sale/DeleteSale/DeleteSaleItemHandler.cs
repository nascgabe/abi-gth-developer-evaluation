using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.DeleteSale
{
    /// <summary>
    /// Handler for processing DeleteSaleItemCommand requests.
    /// </summary>
    public class DeleteSaleItemCommandHandler : IRequestHandler<DeleteSaleItemCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;

        public DeleteSaleItemCommandHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<bool> Handle(DeleteSaleItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);

            if (sale == null)
                return false;

            var saleItem = sale.Items.SingleOrDefault(i => i.Id == request.ItemId);

            if (saleItem == null)
                return false;

            sale.Items.Remove(saleItem);

            sale.TotalValue = sale.Items.Sum(i => i.TotalValue);

            await _saleRepository.UpdateAsync(sale, cancellationToken);

            var itemRemoved = await _saleRepository.DeleteSaleItemAsync(request.ItemId, cancellationToken);

            return itemRemoved;
        }
    }
}