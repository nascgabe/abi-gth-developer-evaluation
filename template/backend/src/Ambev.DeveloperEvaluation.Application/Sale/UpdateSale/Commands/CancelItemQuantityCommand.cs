using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;

/// <summary>
/// Command to update the quantity of an item in a sale.
/// </summary>
public class CancelItemQuantityCommand : IRequest<bool>
{
    public Guid SaleId { get; }
    public Guid ItemId { get; }
    public int Quantity { get; }

    public CancelItemQuantityCommand(Guid saleId, Guid itemId, int quantity)
    {
        SaleId = saleId;
        ItemId = itemId;
        Quantity = quantity;
    }
}