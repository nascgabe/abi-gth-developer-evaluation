using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct
{
    public class DeleteProductProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for DeleteProduct feature
        /// </summary>
        public DeleteProductProfile()
        {
            CreateMap<Guid, Application.Products.DeleteProduct.DeleteProductCommand>()
                .ConstructUsing(id => new Application.Products.DeleteProduct.DeleteProductCommand(id));
        }
    }
}
