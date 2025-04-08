using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Handler for processing GetAllSalesCommand requests
/// </summary>
public class GetAllSalesHandler : IRequestHandler<GetAllSalesCommand, List<GetSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetAllSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetAllSalesCommand request
    /// </summary>
    /// <param name="request">The GetAllSalesCommand request</param>
    /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
    /// <returns>A list of sale results</returns>
    /// <exception cref="DomainException">Thrown when no sales are found</exception>
    public async Task<List<GetSaleResult>> Handle(GetAllSalesCommand request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(cancellationToken);

        if (sales is null || !sales.Any())
        {
            throw new DomainException("No sales were found in the database.");
        }

        return _mapper.Map<List<GetSaleResult>>(sales);
    }
}