using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands
{
    /// <summary>
    /// Command to cancel a sale
    /// </summary>
    public class CancelSaleCommand : IRequest<bool>
    {
        public Guid SaleId { get; }

        public CancelSaleCommand(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}