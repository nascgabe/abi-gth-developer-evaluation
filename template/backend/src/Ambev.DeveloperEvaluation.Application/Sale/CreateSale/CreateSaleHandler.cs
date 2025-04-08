using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public CreateSaleHandler(ISaleRepository saleRepository,
        IProductRepository productRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request.
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        ValidateCommand(command, cancellationToken);

        var sale = _mapper.Map<Domain.Entities.Sale>(command);
        sale.SaleDate = DateTime.UtcNow;

        sale.TotalValue = await ProcessSaleItemsAsync(sale.Items, cancellationToken);
        sale.SaleNumber = await GenerateSaleNumberAsync(cancellationToken);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        return _mapper.Map<CreateSaleResult>(createdSale);
    }

    private async Task<decimal> ProcessSaleItemsAsync(IEnumerable<Domain.Entities.SaleItem> items, CancellationToken cancellationToken)
    {
        decimal totalSaleValue = 0;

        foreach (var item in items)
        {
            var product = await GetValidatedProductAsync(item.ProductId, item.Quantity, cancellationToken);
            UpdateStock(product, item.Quantity, cancellationToken);

            ApplyPricing(item, product);
            totalSaleValue += item.TotalValue;
        }

        return totalSaleValue;
    }

    private async Task<Domain.Entities.Product> GetValidatedProductAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        if (product.Stock < quantity)
            throw new InvalidOperationException($"Product '{product.Title}' does not have enough stock. Available: {product.Stock}, Requested: {quantity}");

        return product;
    }

    private async void UpdateStock(Domain.Entities.Product product, int quantity, CancellationToken cancellationToken)
    {
        product.Stock -= quantity;
        await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);
    }

    private void ApplyPricing(Domain.Entities.SaleItem item, Domain.Entities.Product product)
    {
        item.UnitPrice = product.Price;
        item.ProductName = product.Title ?? "Unknown";

        if (item.Quantity >= 4 && item.Quantity < 10)
        {
            item.Discount = item.UnitPrice * item.Quantity * 0.10m; // 10% de desconto
        }
        else if (item.Quantity >= 10 && item.Quantity <= 20)
        {
            item.Discount = item.UnitPrice * item.Quantity * 0.20m; // 20% de desconto
        }
        else
        {
                item.Discount = 0;
        }

        item.TotalValue = (item.UnitPrice * item.Quantity) - item.Discount;
    }

    private void ValidateCommand(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validationResult = validator.ValidateAsync(command, cancellationToken).GetAwaiter().GetResult();

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    private async Task<string> GenerateSaleNumberAsync(CancellationToken cancellationToken)
    {
        var lastSale = await _saleRepository.GetLastSaleAsync(cancellationToken);
        var nextSaleNumber = (lastSale != null ? int.Parse(lastSale.SaleNumber) : 0) + 1;
        return nextSaleNumber.ToString("D4");
    }
}