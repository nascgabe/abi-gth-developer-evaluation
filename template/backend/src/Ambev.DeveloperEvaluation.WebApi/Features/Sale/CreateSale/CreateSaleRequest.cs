using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class CreateSaleRequest
{
    [JsonIgnore]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public string Client { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public List<SaleItemRequest> Items { get; set; } = new();
}

public class SaleItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
