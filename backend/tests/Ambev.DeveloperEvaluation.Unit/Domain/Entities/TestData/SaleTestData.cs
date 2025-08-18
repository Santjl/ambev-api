using System;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    public static Guid ValidProductId => Guid.Parse("b7dfa2e4-6472-452b-bafa-8ad593985214");
    public static Guid ValidCustomerId => Guid.Parse("cc7a117a-f100-45a9-8ac8-6789ce6eb323");
    public static Guid ValidBranchId => Guid.Parse("8466f144-386e-4fab-bfea-38a4cc27121a");

    public static string ValidSaleNumber => "SALE-001";
    public static string ValidCustomerName => "Customer Test";
    public static string ValidBranchName => "Branch Test";
    public static DateTimeOffset ValidDate => DateTimeOffset.UtcNow;

    public static Sale CreateValidSale()
    {
        return Sale.Create(
            ValidSaleNumber,
            ValidDate,
            ValidCustomerId,
            ValidCustomerName,
            ValidBranchId,
            ValidBranchName
        );
    }

    public static SaleItem CreateValidSaleItem(int quantity = 2, decimal unitPrice = 10m)
    {
        return SaleItem.Create(ValidProductId, "Product Test", quantity, unitPrice);
    }

    public static SaleItem CreateSaleItemWithDiscount(int quantity, decimal unitPrice)
    {
        return SaleItem.Create(ValidProductId, "Product Discount", quantity, unitPrice);
    }

    public static Sale CreateSaleWithItem(int quantity = 2, decimal unitPrice = 10m)
    {
        var sale = CreateValidSale();
        sale.AddItem(ValidProductId, "Product Test", quantity, unitPrice);
        return sale;
    }

    public static Sale CreateSaleWithMultipleItems()
    {
        var sale = CreateValidSale();
        sale.AddItem(ValidProductId, "Product 1", 2, 10m);   // No discount
        sale.AddItem(Guid.NewGuid(), "Product 2", 5, 10m);   // 10% discount
        sale.AddItem(Guid.NewGuid(), "Product 3", 15, 10m);  // 20% discount
        return sale;
    }

    public static Sale CreateCancelledSale()
    {
        var sale = CreateValidSale();
        sale.Cancel();
        return sale;
    }

    public static SaleItem CreateInvalidSaleItem_QuantityZero()
    {
        return SaleItem.Create(ValidProductId, "Product Test", 0, 10m);
    }

    public static SaleItem CreateInvalidSaleItem_QuantityAboveLimit()
    {
        return SaleItem.Create(ValidProductId, "Product Test", 21, 10m);
    }

    public static SaleItem CreateInvalidSaleItem_NegativePrice()
    {
        return SaleItem.Create(ValidProductId, "Product Test", 2, -1m);
    }
}