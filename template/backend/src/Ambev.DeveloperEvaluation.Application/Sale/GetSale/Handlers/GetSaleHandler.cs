using Ambev.DeveloperEvaluation.Application.Sale.GetSale.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.GetSale.Handlers;

/// <summary>
/// Handler for processing GetSaleCommand requests
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public GetSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSaleCommand request
    /// </summary>
    /// <param name="request">The GetSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    /// <exception cref="DomainException">Thrown when validation fails or sale is not found</exception>
    public async Task<GetSaleResult> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        ValidateCommand(request, cancellationToken);

        var sale = await GetSaleByIdAsync(request.Id, cancellationToken);

        return _mapper.Map<GetSaleResult>(sale);
    }

    /// <summary>
    /// Validates the GetSaleCommand to ensure it meets the business rules
    /// </summary>
    /// <param name="command">The GetSaleCommand to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ValidationException">Thrown when the command fails validation</exception>
    private void ValidateCommand(GetSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    /// <summary>
    /// Retrieves the sale entity by ID from the repository
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale entity</returns>
    /// <exception cref="DomainException">Thrown when the sale is not found</exception>
    private async Task<Domain.Entities.Sale> GetSaleByIdAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        if (sale is null)
            throw new DomainException($"Sale with ID {saleId} not found.");

        return sale;
    }
}