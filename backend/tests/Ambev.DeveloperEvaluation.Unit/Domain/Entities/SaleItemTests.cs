using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact(DisplayName = "Should create SaleItem with valid data and no discount")]
    public void Create_ShouldInitializeSaleItemCorrectly_NoDiscount()
    {
        // Arrange & Act
        var item = SaleTestData.CreateValidSaleItem(2, 10m);

        // Assert
        item.ProductName.Should().Be("Product Test");
        item.Quantity.Should().Be(2);
        item.UnitPrice.Should().Be(10m);
        item.DiscountPercent.Should().Be(0m);
        item.Total.Should().Be(20m);
        item.IsCancelled.Should().BeFalse();
    }

    [Fact(DisplayName = "Should create SaleItem with 10% discount")]
    public void Create_ShouldInitializeSaleItemWithTenPercentDiscount()
    {
        // Arrange & Act
        var item = SaleTestData.CreateSaleItemWithDiscount(5, 10m);

        // Assert
        item.Quantity.Should().Be(5);
        item.DiscountPercent.Should().Be(0.10m);
        item.Total.Should().Be(45m); // 5 * 10 = 50, 10% discount = 45
    }

    [Fact(DisplayName = "Should create SaleItem with 20% discount")]
    public void Create_ShouldInitializeSaleItemWithTwentyPercentDiscount()
    {
        // Arrange & Act
        var item = SaleTestData.CreateSaleItemWithDiscount(15, 10m);

        // Assert
        item.Quantity.Should().Be(15);
        item.DiscountPercent.Should().Be(0.20m);
        item.Total.Should().Be(120m); // 15 * 10 = 150, 20% discount = 120
    }

    [Fact(DisplayName = "Should update SaleItem and recalculate discount and total")]
    public void Update_ShouldChangeQuantityAndUnitPriceAndRecalculateTotal()
    {
        // Arrange
        var item = SaleTestData.CreateValidSaleItem(2, 10m);

        // Act
        item.Update(5, 20m);

        // Assert
        item.Quantity.Should().Be(5);
        item.UnitPrice.Should().Be(20m);
        item.DiscountPercent.Should().Be(0.10m);
        item.Total.Should().Be(90m); // 5 * 20 = 100, 10% discount = 90
    }

    [Fact(DisplayName = "Should cancel SaleItem")]
    public void Cancel_ShouldSetIsCancelledTrue()
    {
        // Arrange
        var item = SaleTestData.CreateValidSaleItem();

        // Act
        item.Cancel();

        // Assert
        item.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Should throw when creating SaleItem with quantity less than 1")]
    public void Create_WithQuantityLessThanOne_ShouldThrowDomainException()
    {
        // Arrange & Act
        Action act = () => SaleTestData.CreateInvalidSaleItem_QuantityZero();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Quantity must be at least 1.");
    }

    [Fact(DisplayName = "Should throw when creating SaleItem with quantity greater than 20")]
    public void Create_WithQuantityGreaterThanTwenty_ShouldThrowDomainException()
    {
        // Arrange & Act
        Action act = () => SaleTestData.CreateInvalidSaleItem_QuantityAboveLimit();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Cannot sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Should throw when creating SaleItem with negative unit price")]
    public void Create_WithNegativeUnitPrice_ShouldThrowDomainException()
    {
        // Arrange & Act
        Action act = () => SaleTestData.CreateInvalidSaleItem_NegativePrice();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Unit price cannot be negative.");
    }

    [Fact(DisplayName = "Should throw when updating SaleItem with invalid quantity")]
    public void Update_WithInvalidQuantity_ShouldThrowDomainException()
    {
        // Arrange
        var item = SaleTestData.CreateValidSaleItem();

        // Act
        Action act = () => item.Update(0, 10m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Quantity must be at least 1.");
    }

    [Fact(DisplayName = "Should throw when updating SaleItem with quantity greater than 20")]
    public void Update_WithQuantityGreaterThanTwenty_ShouldThrowDomainException()
    {
        // Arrange
        var item = SaleTestData.CreateValidSaleItem();

        // Act
        Action act = () => item.Update(21, 10m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Cannot sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Should throw when updating SaleItem with negative unit price")]
    public void Update_WithNegativeUnitPrice_ShouldThrowDomainException()
    {
        // Arrange
        var item = SaleTestData.CreateValidSaleItem();

        // Act
        Action act = () => item.Update(2, -1m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Unit price cannot be negative.");
    }
}