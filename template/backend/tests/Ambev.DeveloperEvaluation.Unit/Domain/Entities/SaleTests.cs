using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover validation, item manipulation, and total calculation scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that validation passes when the sale has valid data.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid sale data")]
    public void Given_ValidSaleData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = sale.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when the sale has invalid data.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid sale data")]
    public void Given_InvalidSaleData_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleDate = default,
            Client = "",
            Branch = "",
            Items = new List<SaleItem>(),
            TotalValue = -1
        };

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that the total value of the sale is calculated correctly based on its items.
    /// </summary>
    [Fact(DisplayName = "Total value should be calculated correctly")]
    public void Given_SaleItems_When_CalculatingTotalValue_Then_ShouldReturnCorrectTotal()
    {
        // Arrange
        var sale = new Sale
        {
            Items = new List<SaleItem>
        {
            new SaleItem { UnitPrice = 100m, Quantity = 2, Discount = 20m },
            new SaleItem { UnitPrice = 50m, Quantity = 4, Discount = 10m }
        }
        };

        // Act
        sale.CalculateTotalValue();

        // Assert
        Assert.Equal(370m, sale.TotalValue);
    }

    /// <summary>
    /// Tests that items can be added to the sale.
    /// </summary>
    [Fact(DisplayName = "Items should be added to the sale")]
    public void Given_NewSaleItem_When_AddedToSale_Then_ShouldIncreaseItemCount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Items.Clear();

        // Act
        sale.Items.Add(new SaleItem
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = 100m,
            Discount = 10m
        });

        // Assert
        Assert.NotEmpty(sale.Items);
        Assert.Equal(1, sale.Items.Count);
    }

    /// <summary>
    /// Tests that items can be removed from the sale.
    /// </summary>
    [Fact(DisplayName = "Items should be removed from the sale")]
    public void Given_ExistingSaleItem_When_RemovedFromSale_Then_ShouldDecreaseItemCount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Items.Clear();
        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = 100m,
            Discount = 10m
        };
        sale.Items.Add(item);

        // Act
        sale.Items.Remove(item);

        // Assert
        Assert.Empty(sale.Items);
    }
}