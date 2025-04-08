using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct
{
    public class UpdateProductRequest
    {
        public string? Title { get; set; }
        [JsonIgnore]
        public string? Description { get; set; }
        [JsonIgnore]
        public string? Category { get; set; }
        [JsonIgnore]
        public string? Image { get; set; }
        [JsonIgnore]
        public decimal? Price { get; set; }
        [JsonIgnore]
        public int? Stock { get; set; }
    }
}