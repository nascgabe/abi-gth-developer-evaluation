namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class GetSalesRequest
{
    /// <summary>
    /// The unique identifier of the product to retrieve
    /// </summary>
    public Guid? Id { get; set; }
    /// <summary>
    /// The unique identifier of the product to retrieve
    /// </summary>
    public string? Client { get; set; }
    /// <summary>
    /// The unique identifier of the product to retrieve
    /// </summary>
    public Guid? ProductId { get; set; }
}