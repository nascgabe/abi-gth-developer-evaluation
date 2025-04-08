using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and mock objects.
    /// </summary>
    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _productRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = productId,
                Quantity = 5
            }
        }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product",
            Price = 10m,
            Stock = 100
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "0001",
            SaleDate = command.SaleDate,
            Client = command.Client,
            Branch = command.Branch,
            Items = new List<SaleItem>
        {
            new SaleItem
            {
                ProductId = product.Id,
                Quantity = 5,
                UnitPrice = product.Price,
                Discount = 5m,
                TotalValue = (product.Price * 5) - 5m
            }
        },
            TotalValue = 45m
        };

        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            Client = sale.Client,
            Branch = sale.Branch,
            TotalValue = sale.TotalValue,
            Items = sale.Items.Select(i => new SaleItemResult
            {
                ProductId = i.ProductId,
                ProductName = product.Title,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                TotalValue = i.TotalValue
            }).ToList()
        };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(product.Id, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);
        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(sale.Id);
        createSaleResult.Branch.Should().Be(sale.Branch);
        createSaleResult.TotalValue.Should().Be(sale.TotalValue);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _productRepository.Received(1).UpdateStockAsync(
            Arg.Is(product.Id),
            Arg.Is(95),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var invalidCommand = new CreateSaleCommand();

        // When
        var act = () => _handler.Handle(invalidCommand, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*");
    }

    [Fact(DisplayName = "Given sale request When creating Then updates product stock")]
    public async Task Handle_ValidRequest_UpdatesProductStock()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = productId,
                Quantity = 10
            }
        }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product",
            Price = 10m,
            Stock = 100
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "0001",
            SaleDate = command.SaleDate,
            Client = command.Client,
            Branch = command.Branch,
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = product.Price,
                TotalValue = i.Quantity * product.Price
            }).ToList(),
            TotalValue = command.Items.Sum(i => i.Quantity * product.Price)
        };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(product.Id, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);
        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).UpdateStockAsync(
            Arg.Is(product.Id),
            Arg.Is(90),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given sale request When creating Then applies correct pricing and discounts")]
    public async Task Handle_ValidRequest_AppliesPricingCorrectly()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = productId,
                Quantity = 10
            }
        }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product",
            Price = 100m,
            Stock = 100
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "0001",
            SaleDate = command.SaleDate,
            Client = command.Client,
            Branch = command.Branch,
            Items = new List<SaleItem>
        {
            new SaleItem
            {
                ProductId = product.Id,
                Quantity = 10,
                UnitPrice = product.Price,
                Discount = product.Price * 10 * 0.20m,
                TotalValue = (product.Price * 10) - (product.Price * 10 * 0.20m)
            }
        },
            TotalValue = (product.Price * 10) - (product.Price * 10 * 0.20m)
        };

        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            Client = sale.Client,
            Branch = sale.Branch,
            TotalValue = sale.TotalValue,
            Items = sale.Items.Select(i => new SaleItemResult
            {
                ProductId = i.ProductId,
                ProductName = product.Title,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                TotalValue = i.TotalValue
            }).ToList()
        };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(product.Id, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true); // Estoque atualizado com sucesso
        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.Items.Should().HaveCount(1);

        var item = createSaleResult.Items.First();
        item.Discount.Should().Be(product.Price * 10 * 0.20m);
        item.TotalValue.Should().Be((product.Price * 10) - item.Discount);
        createSaleResult.TotalValue.Should().Be(sale.TotalValue);
    }

    [Fact(DisplayName = "Given invalid product ID When creating sale Then throws DomainException")]
    public async Task Handle_InvalidProductId_ThrowsDomainException()
    {
        // Given
        var invalidProductId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = invalidProductId,
                Quantity = 5
            }
        }
        };

        var sale = new Sale
        {
            SaleDate = command.SaleDate,
            Client = command.Client,
            Branch = command.Branch,
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        _productRepository.GetByIdAsync(invalidProductId, Arg.Any<CancellationToken>()).Returns((Product)null);
        _mapper.Map<Sale>(command).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Product with ID {invalidProductId} not found.");
    }

    [Fact(DisplayName = "Given sale with no items When creating Then throws DomainException")]
    public async Task Handle_NoItemsInSale_ThrowsDomainException()
    {
        // Given
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>()
        };

        _mapper.Map<Sale>(command).Returns(new Sale
        {
            Items = null
        });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Sale must contain at least one item.");
    }

    [Fact(DisplayName = "Given insufficient product stock When creating sale Then throws DomainException")]
    public async Task Handle_InsufficientStock_ThrowsDomainException()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = productId,
                Quantity = 10
            }
        }
        };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(new Product
        {
            Id = productId,
            Title = "Product",
            Price = 100m,
            Stock = 5
        });

        _mapper.Map<Sale>(command).Returns(new Sale
        {
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Product 'Sample Product' does not have enough stock. Available: 5, Requested: 10");
    }

    [Fact(DisplayName = "Given failure to update stock When creating sale Then throws DomainException")]
    public async Task Handle_FailureToUpdateStock_ThrowsDomainException()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = productId,
                Quantity = 5
            }
        }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product",
            Price = 100m,
            Stock = 10
        };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(productId, Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _mapper.Map<Sale>(command).Returns(new Sale
        {
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Failed to update stock for product 'Product'.");
    }

    [Fact(DisplayName = "Given failure to create sale When creating sale Then throws DomainException")]
    public async Task Handle_FailureToCreateSale_ThrowsDomainException()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = productId,
                Quantity = 5
            }
        }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product",
            Price = 100m,
            Stock = 10
        };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(productId, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns((Sale)null);

        _mapper.Map<Sale>(command).Returns(new Sale
        {
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Failed to create the sale. Please try again.");
    }

    [Fact(DisplayName = "Given sale with discountable items When creating sale Then calculates discounts correctly")]
    public async Task Handle_SaleWithDiscount_CalculatesDiscountCorrectly()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            Client = "Gabriel Santos",
            SaleDate = DateTime.UtcNow,
            Branch = "Store A",
            Items = new List<SaleItemCommand>
        {
            new SaleItemCommand
            {
                ProductId = productId,
                Quantity = 10
            }
        }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product",
            Price = 100m,
            Stock = 50
        };

        var sale = new Sale
        {
            SaleDate = command.SaleDate,
            Client = command.Client,
            Branch = command.Branch,
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = product.Price,
                Discount = product.Price * 10 * 0.20m,
                TotalValue = (product.Price * 10) - (product.Price * 10 * 0.20m)
            }).ToList(),
            TotalValue = (product.Price * 10) - (product.Price * 10 * 0.20m)
        };

        var createSaleResult = new CreateSaleResult
        {
            Id = Guid.NewGuid(),
            SaleDate = sale.SaleDate,
            Client = sale.Client,
            Branch = sale.Branch,
            TotalValue = sale.TotalValue,
            Items = sale.Items.Select(i => new SaleItemResult
            {
                ProductId = i.ProductId,
                ProductName = product.Title,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                TotalValue = i.TotalValue
            }).ToList()
        };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(productId, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(createSaleResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.TotalValue.Should().Be(createSaleResult.TotalValue);

        var item = result.Items.First();
        item.Discount.Should().Be(product.Price * 10 * 0.20m);
    }
}