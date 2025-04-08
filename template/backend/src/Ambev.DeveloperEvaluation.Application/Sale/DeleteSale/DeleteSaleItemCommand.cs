using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.DeleteSale
{
    /// <summary>
    /// Command to delete a sale item.
    /// </summary>
    public class DeleteSaleItemCommand : IRequest<bool>
    {
        public Guid SaleId { get; }
        public Guid ItemId { get; }

        public DeleteSaleItemCommand(Guid saleId, Guid itemId)
        {
            SaleId = saleId;
            ItemId = itemId;
        }
    }
}