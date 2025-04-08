namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateItemQuantity;

/// <summary>
/// Payload for updating the quantity of an item in a sale.
/// </summary>
public class UpdateItemQuantityRequest
{
    public int Quantity { get; set; }

    public UpdateItemQuantityRequest(int quantity)
    {
        Quantity = quantity;
    }
}