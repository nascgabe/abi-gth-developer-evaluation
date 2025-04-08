namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Result returned after a sale is successfully created.
/// </summary>
public class CreateSaleResult
{
    public Guid Id { get; set; }
    public DateTime SaleDate { get; set; }
    public string Client { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string SaleNumber { get; set; } = string.Empty;
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