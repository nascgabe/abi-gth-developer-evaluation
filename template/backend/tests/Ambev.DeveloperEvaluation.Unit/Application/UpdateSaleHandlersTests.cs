using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;
using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Handlers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleHandlersTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CancelSaleHandler> _cancelSaleLogger;
    private readonly ILogger<CancelItemQuantityHandler> _cancelItemLogger;
    private readonly CancelSaleHandler _cancelSaleHandler;
    private readonly CancelItemQuantityHandler _cancelItemHandler;

    public UpdateSaleHandlersTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _cancelSaleLogger = Substitute.For<ILogger<CancelSaleHandler>>();
        _cancelItemLogger = Substitute.For<ILogger<CancelItemQuantityHandler>>();

        _cancelSaleHandler = new CancelSaleHandler(_saleRepository, _productRepository, _cancelSaleLogger);
        _cancelItemHandler = new CancelItemQuantityHandler(_saleRepository, _productRepository, _cancelItemLogger);
    }

    [Fact(DisplayName = "Given a valid sale ID When cancelling Then sale is cancelled successfully")]
    public async Task CancelSale_ValidSale_CancelsSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>
        {
            new SaleItem { ProductId = productId, Quantity = 5 }
        }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product",
            Stock = 50
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(productId, product.Stock + 5, Arg.Any<CancellationToken>())
            .Returns(true);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _cancelSaleHandler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        sale.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Given a sale ID for an already cancelled sale When cancelling Then throws DomainException")]
    public async Task CancelSale_AlreadyCancelledSale_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = true,
            Items = new List<SaleItem>()
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var act = async () => await _cancelSaleHandler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Sale with ID {saleId} is already cancelled.");
    }

    [Fact(DisplayName = "Given an invalid sale ID When cancelling Then throws DomainException")]
    public async Task CancelSale_InvalidSaleId_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale)null);

        // Act
        var act = async () => await _cancelSaleHandler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Sale with ID {saleId} not found.");
    }

    [Fact(DisplayName = "Given a valid sale item ID and quantity When updating quantity Then updates successfully")]
    public async Task CancelItemQuantity_ValidItemAndQuantity_UpdatesSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new SaleItem { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 10, UnitPrice = 100 }
            }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Product { Id = sale.Items.First().ProductId, Stock = 50 });
        _productRepository.UpdateStockAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _cancelItemHandler.Handle(
            new CancelItemQuantityCommand(saleId, itemId, 5), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        sale.Items.First().Quantity.Should().Be(5);
    }

    [Fact(DisplayName = "Given a quantity of 0 When updating item Then item is removed successfully")]
    public async Task CancelItemQuantity_QuantityZero_RemovesItemSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new SaleItem { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 10 }
            }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteSaleItemAsync(itemId, Arg.Any<CancellationToken>()).Returns(true);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _cancelItemHandler.Handle(
            new CancelItemQuantityCommand(saleId, itemId, 0), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        sale.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given an invalid sale item ID When updating quantity Then throws DomainException")]
    public async Task CancelItemQuantity_InvalidItemId_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>()
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var act = async () => await _cancelItemHandler.Handle(
            new CancelItemQuantityCommand(saleId, itemId, 5), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"The sale item with ID {itemId} was not found in the sale.");
    }

    [Fact(DisplayName = "Given valid sale ID When cancelling Then restores product stock")]
    public async Task CancelSale_RestoresProductStockSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new SaleItem { ProductId = productId, Quantity = 5 }
            }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product A",
            Stock = 50
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(productId, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(true);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _cancelSaleHandler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        product.Stock.Should().Be(55);
    }

    [Fact(DisplayName = "Given failure to update product stock When cancelling Then throws DomainException")]
    public async Task CancelSale_FailureToUpdateStock_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new SaleItem { ProductId = productId, Quantity = 5 }
            }
        };

        var product = new Product
        {
            Id = productId,
            Title = "Product A",
            Stock = 50
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateStockAsync(productId, Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await _cancelSaleHandler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Failed to update stock for product '{product.Title}'.");
    }

    [Fact(DisplayName = "Given invalid item quantity When updating Then throws DomainException")]
    public async Task CancelItemQuantity_InvalidQuantity_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new SaleItem { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 10 }
            }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var act = async () => await _cancelItemHandler.Handle(
            new CancelItemQuantityCommand(saleId, itemId, 21), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid quantity: 21. Quantity must be between 1 and 20.");
    }

    [Fact(DisplayName = "Given removal of all items When updating Then marks sale as empty")]
    public async Task CancelItemQuantity_RemovalOfAllItems_MarksSaleAsEmpty()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new SaleItem { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 10 }
            }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteSaleItemAsync(itemId, Arg.Any<CancellationToken>()).Returns(true);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _cancelItemHandler.Handle(
            new CancelItemQuantityCommand(saleId, itemId, 0), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        sale.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given sale already cancelled When updating item Then throws DomainException")]
    public async Task CancelItemQuantity_AlreadyCancelledSale_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = true,
            Items = new List<SaleItem>
            {
                new SaleItem { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 10 }
            }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var act = async () => await _cancelItemHandler.Handle(
            new CancelItemQuantityCommand(saleId, itemId, 5), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"The sale with ID {saleId} is already cancelled. No changes are allowed.");
    }
}