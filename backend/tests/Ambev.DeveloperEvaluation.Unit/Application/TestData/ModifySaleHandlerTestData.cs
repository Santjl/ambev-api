using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Centraliza a geração de dados de teste para ModifySaleHandler usando Bogus.
/// Gera comandos válidos e inválidos para todos os cenários.
/// </summary>
public static class ModifySaleHandlerTestData
{
    private static readonly Faker<ModifySaleItemCommand> itemFaker = new Faker<ModifySaleItemCommand>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

    /// <summary>
    /// Gera um comando válido para modificar uma venda existente.
    /// </summary>
    public static ModifySaleCommand GenerateValidCommand(Guid? saleId = null, int itemCount = 2)
    {
        return new ModifySaleCommand
        {
            SaleId = saleId ?? Guid.NewGuid(),
            Items = itemFaker.Generate(itemCount)
        };
    }

    /// <summary>
    /// Gera um comando com SaleId inexistente.
    /// </summary>
    public static ModifySaleCommand GenerateCommandWithNonExistentSaleId()
    {
        return GenerateValidCommand(Guid.NewGuid());
    }

    /// <summary>
    /// Gera um comando para cancelar um item (Quantity = 0).
    /// </summary>
    public static ModifySaleCommand GenerateCommandToCancelItem(Guid saleId, Guid productId)
    {
        var item = new ModifySaleItemCommand { ProductId = productId, Quantity = 0 };
        return new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand> { item }
        };
    }

    /// <summary>
    /// Gera um comando com produto inexistente.
    /// </summary>
    public static ModifySaleCommand GenerateCommandWithNonExistentProduct(Guid saleId)
    {
        var item = new ModifySaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 };
        return new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand> { item }
        };
    }

    /// <summary>
    /// Gera um comando para venda cancelada.
    /// </summary>
    public static ModifySaleCommand GenerateCommandForCancelledSale(Guid saleId)
    {
        return GenerateValidCommand(saleId);
    }
}