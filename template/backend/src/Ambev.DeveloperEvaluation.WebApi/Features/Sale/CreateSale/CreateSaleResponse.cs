namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class CreateSaleResponse
{
    public Guid Id { get; set; }
    public DateTime SaleDate { get; set; }
    public string Client { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public List<SaleItemResponse> Items { get; set; } = new();
}

public class SaleItemResponse
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalValue { get; set; }
}