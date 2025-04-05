namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

public class CreateProductRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public RatingRequest Rating { get; set; } = new();
}

public class RatingRequest
{
    public double Rate { get; set; }
    public int Count { get; set; }
}