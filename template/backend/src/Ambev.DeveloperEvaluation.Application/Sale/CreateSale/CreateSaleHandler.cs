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
    /// <param name="command">The sale creation command containing sale details</param>
    /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        ValidateCommand(command);

        var sale = _mapper.Map<Domain.Entities.Sale>(command);
        sale.SaleDate = DateTime.UtcNow;

        sale.TotalValue = await ProcessSaleItemsAsync(sale.Items, cancellationToken);
        sale.SaleNumber = await GenerateSaleNumberAsync(cancellationToken);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        return _mapper.Map<CreateSaleResult>(createdSale);
    }

    /// <summary>
    /// Processes the sale items by validating products, updating stock, applying pricing, and calculating total sale value.
    /// </summary>
    /// <param name="items">The list of items in the sale</param>
    /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
    /// <returns>The total value of the sale after processing all items</returns>
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

    /// <summary>
    /// Validates the product ID and stock availability for the specified quantity.
    /// </summary>
    /// <param name="productId">The unique identifier of the product</param>
    /// <param name="quantity">The quantity requested for the sale</param>
    /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
    /// <returns>The validated product entity</returns>
    /// <exception cref="DomainException">Thrown when the product is not found or has insufficient stock</exception>
    private async Task<Domain.Entities.Product> GetValidatedProductAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new DomainException($"Product with ID {productId} not found.");

        if (product.Stock < quantity)
            throw new DomainException($"Product '{product.Title}' does not have enough stock. Available: {product.Stock}, Requested: {quantity}");

        return product;
    }

    /// <summary>
    /// Updates the stock of a product in the repository based on the quantity sold.
    /// </summary>
    /// <param name="product">The product to update</param>
    /// <param name="quantity">The quantity sold</param>
    /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
    /// <exception cref="DomainException">Thrown when the stock update fails</exception>
    private async void UpdateStock(Domain.Entities.Product product, int quantity, CancellationToken cancellationToken)
    {
        product.Stock -= quantity;
        var updated = await _productRepository.UpdateStockAsync(product.Id, product.Stock, cancellationToken);

        if (!updated)
            throw new DomainException($"Failed to update stock for product '{product.Title}'.");
    }

    /// <summary>
    /// Applies pricing, including discounts, to a sale item based on product details.
    /// </summary>
    /// <param name="item">The sale item to apply pricing to</param>
    /// <param name="product">The product associated with the sale item</param>
    private void ApplyPricing(Domain.Entities.SaleItem item, Domain.Entities.Product product)
    {
        item.UnitPrice = product.Price;
        item.ProductName = product.Title ?? "Unknown";

        if (item.Quantity >= 4 && item.Quantity < 10)
        {
            item.Discount = item.UnitPrice * item.Quantity * 0.10m;
        }
        else if (item.Quantity >= 10 && item.Quantity <= 20)
        {
            item.Discount = item.UnitPrice * item.Quantity * 0.20m;
        }
        else
        {
            item.Discount = 0;
        }

        item.TotalValue = (item.UnitPrice * item.Quantity) - item.Discount;
    }

    /// <summary>
    /// Validates the CreateSale command to ensure it meets the business rules.
    /// </summary>
    /// <param name="command">The sale creation command</param>
    /// <exception cref="ValidationException">Thrown when the command validation fails</exception>
    private void ValidateCommand(CreateSaleCommand command)
    {
        var validator = new CreateSaleValidator();
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    /// <summary>
    /// Generates the next sale number based on the last recorded sale.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
    /// <returns>The next sale number in sequence</returns>
    private async Task<string> GenerateSaleNumberAsync(CancellationToken cancellationToken)
    {
        var lastSale = await _saleRepository.GetLastSaleAsync(cancellationToken);
        var nextSaleNumber = (lastSale != null ? int.Parse(lastSale.SaleNumber) : 0) + 1;
        return nextSaleNumber.ToString("D4");
    }
}