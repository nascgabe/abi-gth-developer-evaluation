namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Result for retrieving a sale by its ID
/// </summary>
public class GetSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string Client { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public List<SaleItemResult> Items { get; set; } = new();
}

public class SaleItemResult
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalValue { get; set; }
}