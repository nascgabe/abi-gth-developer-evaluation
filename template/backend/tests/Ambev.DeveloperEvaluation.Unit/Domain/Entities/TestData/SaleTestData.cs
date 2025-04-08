using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Configures the Faker to generate valid SaleItem entities.
    /// The generated sale items will have valid:
    /// - ProductId (random GUID)
    /// - Quantity (within allowed range)
    /// - UnitPrice (positive decimal value)
    /// - Discount (calculated based on rules)
    /// </summary>
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(10, 100))
        .RuleFor(i => i.Discount, (f, item) =>
        {
            if (item.Quantity >= 4 && item.Quantity < 10)
                return item.UnitPrice * item.Quantity * 0.10m;
            if (item.Quantity >= 10 && item.Quantity <= 20)
                return item.UnitPrice * item.Quantity * 0.20m;
            return 0;
        })
        .RuleFor(i => i.TotalValue, (f, item) => (item.UnitPrice * item.Quantity) - item.Discount);

    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - SaleDate (current date)
    /// - Client (random name)
    /// - Branch (random location name)
    /// - Items (list of valid SaleItem entities)
    /// - TotalValue (calculated based on items)
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
     .RuleFor(s => s.SaleDate, f => f.Date.Between(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow))
     .RuleFor(s => s.Client, f => f.Name.FullName())
     .RuleFor(s => s.Branch, f => f.Company.CompanyName())
     .RuleFor(s => s.Items, f => SaleItemFaker.Generate(f.Random.Int(1, 5)))
     .RuleFor(s => s.TotalValue, (f, sale) => sale.Items.Sum(i => i.TotalValue));

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// </summary>
    /// <returns>A valid Sale entity.</returns>
    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid Sale entity for testing error cases.
    /// </summary>
    /// <returns>An invalid Sale entity.</returns>
    public static Sale GenerateInvalidSale()
    {
        return new Sale
        {
            SaleDate = default,
            Client = "",
            Branch = "",
            Items = new List<SaleItem>(),
            TotalValue = -1
        };
    }

    /// <summary>
    /// Generates a valid SaleItem entity with randomized data.
    /// </summary>
    /// <returns>A valid SaleItem entity.</returns>
    public static SaleItem GenerateValidSaleItem()
    {
        return SaleItemFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid SaleItem entity for testing error cases.
    /// </summary>
    /// <returns>An invalid SaleItem entity.</returns>
    public static SaleItem GenerateInvalidSaleItem()
    {
        return new SaleItem
        {
            ProductId = Guid.Empty,
            Quantity = 0,
            UnitPrice = -1,
            Discount = -1,
            TotalValue = -1
        };
    }
}