using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Command for retrieving all sales
/// </summary>
public record GetAllSalesCommand : IRequest<List<GetSaleResult>>;
