using Ambev.DeveloperEvaluation.WebApi.Features.Sale.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Profile for mapping sale-related objects in the WebAPI
/// </summary>
public class SalesApiProfile : Profile
{
    public SalesApiProfile()
    {
        CreateMap<Application.Sales.GetSale.GetSaleResult, GetSalesResponse.SaleResponse>();
        CreateMap<Application.Sales.GetSale.SaleItemResult, GetSalesResponse.SaleItemResponse>();
    }
}