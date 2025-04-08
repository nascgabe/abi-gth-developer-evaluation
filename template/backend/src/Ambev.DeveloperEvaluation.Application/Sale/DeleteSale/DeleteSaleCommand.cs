using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.DeleteSale
{
    /// <summary>
    /// Command to delete a sale.
    /// </summary>
    public class DeleteSaleCommand : IRequest<bool>
    {
        public Guid SaleId { get; }

        public DeleteSaleCommand(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}