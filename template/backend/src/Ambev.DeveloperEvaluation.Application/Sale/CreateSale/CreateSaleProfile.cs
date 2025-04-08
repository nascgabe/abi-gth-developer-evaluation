using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Domain.Entities.Sale>();
        CreateMap<SaleItemCommand, SaleItem>();
        CreateMap<Domain.Entities.Sale, CreateSaleResult>()
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue));
        CreateMap<SaleItem, SaleItemResult>();
    }
}