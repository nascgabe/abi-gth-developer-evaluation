using Ambev.DeveloperEvaluation.Application.Sale.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteSaleHandlersTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteSaleHandler> _deleteSaleLogger;
    private readonly ILogger<DeleteSaleItemHandler> _deleteSaleItemLogger;
    private readonly DeleteSaleHandler _deleteSaleHandler;
    private readonly DeleteSaleItemHandler _deleteSaleItemHandler;

    public DeleteSaleHandlersTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _deleteSaleLogger = Substitute.For<ILogger<DeleteSaleHandler>>();
        _deleteSaleItemLogger = Substitute.For<ILogger<DeleteSaleItemHandler>>();

        _deleteSaleHandler = new DeleteSaleHandler(_saleRepository, _deleteSaleLogger);
        _deleteSaleItemHandler = new DeleteSaleItemHandler(_saleRepository, _deleteSaleItemLogger);
    }

    [Fact(DisplayName = "Given valid sale ID When deleting sale Then deletes successfully")]
    public async Task DeleteSale_ValidSaleId_DeletesSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given invalid sale ID When deleting sale Then throws DomainException")]
    public async Task DeleteSale_InvalidSaleId_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale)null);

        // Act
        var act = async () => await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"The sale with ID {saleId} does not exist.");
    }

    [Fact(DisplayName = "Given failure to delete sale When deleting sale Then throws DomainException")]
    public async Task DeleteSale_FailureToDelete_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Failed to delete the sale with ID {saleId}. Please try again.");
    }

    [Fact(DisplayName = "Given valid sale item ID When deleting item Then deletes successfully")]
    public async Task DeleteSaleItem_ValidItemId_DeletesSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            Items = new List<SaleItem> { new SaleItem { Id = itemId } }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteSaleItemAsync(itemId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _deleteSaleItemHandler.Handle(new DeleteSaleItemCommand(saleId, itemId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        sale.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given invalid sale item ID When deleting item Then throws DomainException")]
    public async Task DeleteSaleItem_InvalidItemId_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            Items = new List<SaleItem>()
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var act = async () => await _deleteSaleItemHandler.Handle(new DeleteSaleItemCommand(saleId, itemId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"The sale item with ID {itemId} does not exist.");
    }

    [Fact(DisplayName = "Given failure to delete sale item When deleting item Then throws DomainException")]
    public async Task DeleteSaleItem_FailureToDeleteItem_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            Items = new List<SaleItem> { new SaleItem { Id = itemId } }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteSaleItemAsync(itemId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await _deleteSaleItemHandler.Handle(new DeleteSaleItemCommand(saleId, itemId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Failed to delete the sale item with ID {itemId}. Please try again.");
    }

    [Fact(DisplayName = "Given sale with items When deleting Then ensures associated items are handled")]
    public async Task DeleteSale_WithAssociatedItems_EnsuresItemsHandled()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            Items = new List<SaleItem> { new SaleItem { Id = itemId } }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _saleRepository.Received(1).DeleteAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given cancelled sale When deleting Then deletes without restoring stock")]
    public async Task DeleteCancelledSale_DeletesWithoutRestoringStock()
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
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _productRepository.DidNotReceiveWithAnyArgs().UpdateStockAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given failure in repository When deleting sale Then throws DomainException")]
    public async Task DeleteSale_RepositoryFailure_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Failed to delete the sale with ID {saleId}. Please try again.");
    }

    [Fact(DisplayName = "Given invalid product for item When deleting item Then deletes successfully")]
    public async Task DeleteSaleItem_InvalidProduct_DeletesSuccessfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            Items = new List<SaleItem> { new SaleItem { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 5 } }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteSaleItemAsync(itemId, Arg.Any<CancellationToken>()).Returns(true);
        _productRepository.GetByIdAsync(sale.Items.First().ProductId, Arg.Any<CancellationToken>()).Returns((Product)null);

        // Act
        var result = await _deleteSaleItemHandler.Handle(new DeleteSaleItemCommand(saleId, itemId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        sale.Items.Should().BeEmpty();
    }
}