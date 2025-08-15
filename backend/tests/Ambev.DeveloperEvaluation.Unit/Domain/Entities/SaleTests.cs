using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Should create Sale with valid data")]
    public void Create_ShouldInitializeSaleCorrectly()
    {
        // Arrange & Act
        var sale = SaleTestData.CreateValidSale();

        // Assert
        sale.Number.Should().Be(SaleTestData.ValidSaleNumber);
        sale.IsCancelled.Should().BeFalse();
        sale.Items.Should().BeEmpty();
        sale.Total.Should().Be(0);
    }

    [Fact(DisplayName = "Should throw when creating Sale with empty number")]
    public void Create_WithEmptyNumber_ShouldThrowDomainException()
    {
        // Arrange & Act
        Action act = () => Sale.Create("", SaleTestData.ValidDate, SaleTestData.ValidCustomerId, SaleTestData.ValidCustomerName, SaleTestData.ValidBranchId, SaleTestData.ValidBranchName);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Sale number is required.");
    }

    [Fact(DisplayName = "Should add item with no discount and update total")]
    public void AddItem_WithNoDiscount_ShouldAddItemAndUpdateTotal()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();

        // Act
        sale.AddItem(SaleTestData.ValidProductId, "Product Test", 2, 10m);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.Total.Should().Be(20m);
        sale.Items.First().DiscountPercent.Should().Be(0m);
    }

    [Fact(DisplayName = "Should add item with 10% discount and update total")]
    public void AddItem_WithTenPercentDiscount_ShouldAddItemAndUpdateTotal()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();

        // Act
        sale.AddItem(SaleTestData.ValidProductId, "Product Test", 5, 10m);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.Total.Should().Be(45m); // 5 * 10 = 50, 10% discount = 45
        sale.Items.First().DiscountPercent.Should().Be(0.10m);
    }

    [Fact(DisplayName = "Should add item with 20% discount and update total")]
    public void AddItem_WithTwentyPercentDiscount_ShouldAddItemAndUpdateTotal()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();

        // Act
        sale.AddItem(SaleTestData.ValidProductId, "Product Test", 15, 10m);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.Total.Should().Be(120m); // 15 * 10 = 150, 20% discount = 120
        sale.Items.First().DiscountPercent.Should().Be(0.20m);
    }

    [Fact(DisplayName = "Should update item and recalculate discount and total")]
    public void UpdateItem_ShouldUpdateQuantityAndTotalWithDiscount()
    {
        // Arrange
        var sale = SaleTestData.CreateSaleWithItem(2, 10m);
        var item = sale.Items.First();

        // Act
        sale.UpdateItem(item.Id, 5, 10m);

        // Assert
        item.Quantity.Should().Be(5);
        item.DiscountPercent.Should().Be(0.10m);
        sale.Total.Should().Be(45m);
    }

    [Fact(DisplayName = "Should cancel item and update total")]
    public void CancelItem_ShouldSetItemCancelledAndUpdateTotal()
    {
        // Arrange
        var sale = SaleTestData.CreateSaleWithItem(2, 10m);
        var item = sale.Items.First();

        // Act
        sale.CancelItem(item.Id);

        // Assert
        item.IsCancelled.Should().BeTrue();
        sale.Total.Should().Be(0m);
    }

    [Fact(DisplayName = "Should remove item and update total")]
    public void RemoveItem_ShouldRemoveItemAndUpdateTotal()
    {
        // Arrange
        var sale = SaleTestData.CreateSaleWithItem(2, 10m);
        var item = sale.Items.First();

        // Act
        sale.RemoveItem(item.Id);

        // Assert
        sale.Items.Should().BeEmpty();
        sale.Total.Should().Be(0m);
    }

    [Fact(DisplayName = "Should calculate total with multiple items and discounts")]
    public void Total_ShouldSumAllItemsWithDiscounts()
    {
        // Arrange
        var sale = SaleTestData.CreateSaleWithMultipleItems();

        // Act
        var total = sale.Total;

        // Assert
        // Product 1: 2 * 10 = 20 (no discount)
        // Product 2: 5 * 10 = 50, 10% discount = 45
        // Product 3: 15 * 10 = 150, 20% discount = 120
        total.Should().Be(20m + 45m + 120m);
    }

    [Fact(DisplayName = "Should cancel sale")]
    public void Cancel_ShouldSetSaleAsCancelled()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();

        // Act
        sale.Cancel();

        // Assert
        sale.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Should throw when adding item to cancelled sale")]
    public void AddItem_WhenSaleIsCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.CreateCancelledSale();

        // Act
        Action act = () => sale.AddItem(SaleTestData.ValidProductId, "Product Test", 2, 10m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Sale is cancelled.");
    }

    [Fact(DisplayName = "Should throw when updating item in cancelled sale")]
    public void UpdateItem_WhenSaleIsCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.CreateSaleWithItem(2, 10m);
        sale.Cancel();
        var item = sale.Items.First();

        // Act
        Action act = () => sale.UpdateItem(item.Id, 5, 10m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Sale is cancelled.");
    }

    [Fact(DisplayName = "Should throw when cancelling item in cancelled sale")]
    public void CancelItem_WhenSaleIsCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.CreateSaleWithItem(2, 10m);
        sale.Cancel();
        var item = sale.Items.First();

        // Act
        Action act = () => sale.CancelItem(item.Id);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Sale is cancelled.");
    }

    [Fact(DisplayName = "Should throw when removing item in cancelled sale")]
    public void RemoveItem_WhenSaleIsCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.CreateSaleWithItem(2, 10m);
        sale.Cancel();
        var item = sale.Items.First();

        // Act
        Action act = () => sale.RemoveItem(item.Id);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Sale is cancelled.");
    }

    [Fact(DisplayName = "Should throw when updating non-existent item")]
    public void UpdateItem_WithInvalidItemId_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();

        // Act
        Action act = () => sale.UpdateItem(Guid.NewGuid(), 5, 10m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Item not found.");
    }

    [Fact(DisplayName = "Should throw when cancelling non-existent item")]
    public void CancelItem_WithInvalidItemId_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();

        // Act
        Action act = () => sale.CancelItem(Guid.NewGuid());

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Item not found.");
    }
}