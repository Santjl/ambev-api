using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Centralizes test data generation for CreateSaleHandler using Bogus.
/// Provides valid and invalid commands for all scenarios.
/// </summary>
public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleItemCommand> itemFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

    /// <summary>
    /// Generates a valid command to create a sale.
    /// </summary>
    public static CreateSaleCommand GenerateValidCommand(int itemCount = 1)
    {
        return new CreateSaleCommand
        {
            Number = $"SALE-{new Random().Next(100, 999)}",
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = itemFaker.Generate(itemCount)
        };
    }

    /// <summary>
    /// Generates an invalid command (empty number).
    /// </summary>
    public static CreateSaleCommand GenerateInvalidCommand()
    {
        var cmd = GenerateValidCommand();
        cmd.Number = "";
        return cmd;
    }

    /// <summary>
    /// Generates a command with a non-existent CustomerId.
    /// </summary>
    public static CreateSaleCommand GenerateCommandWithNonExistentCustomer()
    {
        return GenerateValidCommand();
    }

    /// <summary>
    /// Generates a command with a non-existent BranchId.
    /// </summary>
    public static CreateSaleCommand GenerateCommandWithNonExistentBranch()
    {
        return GenerateValidCommand();
    }

    /// <summary>
    /// Generates a command with a non-existent ProductId.
    /// </summary>
    public static CreateSaleCommand GenerateCommandWithNonExistentProduct()
    {
        return GenerateValidCommand();
    }
}