using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ModifySaleHandlerTestData
{
    private static readonly Faker<ModifySaleItemCommand> itemFaker = new Faker<ModifySaleItemCommand>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

    public static ModifySaleCommand GenerateValidCommand(Guid? saleId = null, int itemCount = 2)
    {
        return new ModifySaleCommand
        {
            SaleId = saleId ?? Guid.NewGuid(),
            Items = itemFaker.Generate(itemCount)
        };
    }

    public static ModifySaleCommand GenerateCommandWithNonExistentSaleId()
    {
        return GenerateValidCommand(Guid.NewGuid());
    }

    public static ModifySaleCommand GenerateCommandToCancelItem(Guid saleId, Guid productId)
    {
        var item = new ModifySaleItemCommand { ProductId = productId, Quantity = 0 };
        return new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand> { item }
        };
    }
    public static ModifySaleCommand GenerateCommandWithNonExistentProduct(Guid saleId)
    {
        var item = new ModifySaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 };
        return new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand> { item }
        };
    }
    public static ModifySaleCommand GenerateCommandForCancelledSale(Guid saleId)
    {
        return GenerateValidCommand(saleId);
    }
}