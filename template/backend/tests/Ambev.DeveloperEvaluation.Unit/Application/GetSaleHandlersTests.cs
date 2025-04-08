using Ambev.DeveloperEvaluation.Application.Sale.GetSale.Commands;
using Ambev.DeveloperEvaluation.Application.Sale.GetSale.Handlers;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleHandlersTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _getSaleHandler;
    private readonly GetAllSalesHandler _getAllSalesHandler;

    public GetSaleHandlersTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();

        _getSaleHandler = new GetSaleHandler(_saleRepository, _mapper);
        _getAllSalesHandler = new GetAllSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid sale ID When retrieving sale Then returns sale details")]
    public async Task GetSale_ValidSaleId_ReturnsSaleDetails()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId, Client = "Gabriel Santos" };
        var saleResult = new GetSaleResult { Id = saleId, Client = "Gabriel Santos" };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(saleResult);

        // Act
        var result = await _getSaleHandler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleResult.Id);
        result.Client.Should().Be(saleResult.Client);
    }

    [Fact(DisplayName = "Given invalid sale ID When retrieving sale Then throws DomainException")]
    public async Task GetSale_InvalidSaleId_ThrowsDomainException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale)null);

        // Act
        var act = async () => await _getSaleHandler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Sale with ID {saleId} not found.");
    }

    [Fact(DisplayName = "Given sales exist When retrieving all Then returns sale details")]
    public async Task GetAllSales_SalesExist_ReturnsSaleDetails()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new Sale { Id = Guid.NewGuid(), Client = "Gabriel Santos" },
            new Sale { Id = Guid.NewGuid(), Client = "Santos Gabriel" }
        };
        var salesResults = new List<GetSaleResult>
        {
            new GetSaleResult { Id = sales[0].Id, Client = sales[0].Client },
            new GetSaleResult { Id = sales[1].Id, Client = sales[1].Client }
        };

        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(sales);
        _mapper.Map<List<GetSaleResult>>(sales).Returns(salesResults);

        // Act
        var result = await _getAllSalesHandler.Handle(new GetAllSalesCommand(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Client.Should().Be("Gabriel Santos");
        result.Last().Client.Should().Be("Santos Gabriel");
    }

    [Fact(DisplayName = "Given no sales exist When retrieving all Then throws DomainException")]
    public async Task GetAllSales_NoSalesExist_ThrowsDomainException()
    {
        // Arrange
        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Sale>());

        // Act
        var act = async () => await _getAllSalesHandler.Handle(new GetAllSalesCommand(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("No sales were found in the database.");
    }
}
