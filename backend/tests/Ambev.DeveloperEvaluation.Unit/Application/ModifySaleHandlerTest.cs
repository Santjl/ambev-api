using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;
using static Ambev.DeveloperEvaluation.Application.Common.Ports.DTos;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ModifySaleHandlerTest
{
    private readonly Mock<ISaleRepository> _saleRepository = new();
    private readonly Mock<IProductGateway> _productGateway = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IMessageBus> _messageBus = new();

    private ModifySaleHandler CreateHandler() =>
        new ModifySaleHandler(
            _saleRepository.Object,
            _productGateway.Object,
            _mapper.Object,
            _messageBus.Object
        );

    [Fact(DisplayName = "Should modify sale successfully when command data is valid")]
    public async Task Handle_ShouldModifySale_WhenValidCommand()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = SaleTestData.ValidProductId;
        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        var sale = SaleTestData.CreateValidSale();
        typeof(Sale).GetProperty("Id")?.SetValue(sale, saleId);

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _productGateway.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(productId, "Product Test", 10m));
        _messageBus.Setup(x => x.PublishMessagesAsync(It.IsAny<IEnumerable<IntegrationMessage>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapper.Setup(m => m.Map<ModifySaleResult>(It.IsAny<object>())).Returns(new ModifySaleResult { Id = saleId });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(saleId);
        _saleRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _messageBus.Verify(x => x.PublishMessagesAsync(It.Is<IEnumerable<IntegrationMessage>>(msgs =>
                msgs.Count() == 1 &&
                msgs.First().Name == "sales.sale.modified" &&
                msgs.First().Payload is SaleModifiedEvent),
                It.IsAny<CancellationToken>()),Times.Once);
        sale.Items.Should().ContainSingle(i => i.ProductId == productId && i.Quantity == 2);
    }

    [Fact(DisplayName = "Should throw exception when SaleId does not exist")]
    public async Task Handle_ShouldThrow_WhenSaleIdDoesNotExist()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = SaleTestData.ValidProductId, Quantity = 2 }
            }
        };

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync((Sale)null!);

        // Act
        Func<Task> act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Should throw exception when trying to modify a cancelled sale")]
    public async Task Handle_ShouldThrowException_WhenSaleIsCancelled()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = SaleTestData.ValidProductId, Quantity = 2 }
            }
        };

        var sale = SaleTestData.CreateCancelledSale();
        typeof(Sale).GetProperty("Id")?.SetValue(sale, saleId);

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);

        // Act
        Func<Task> act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>();
    }

    [Fact(DisplayName = "Should throw exception when product does not exist")]
    public async Task Handle_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = SaleTestData.ValidProductId;
        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        var sale = SaleTestData.CreateValidSale();
        typeof(Sale).GetProperty("Id")?.SetValue(sale, saleId);

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _productGateway.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto)null!);

        // Act
        Func<Task> act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>();
    }

    [Fact(DisplayName = "Should cancel item when quantity is updated to zero")]
    public async Task Handle_ShouldCancelItem_WhenQuantityZero()
    {
        // Arrange
        var productId = SaleTestData.ValidProductId;
        var newProductId = Guid.NewGuid();
        var sale = SaleTestData.CreateSaleWithMultipleItems();

        var command = new ModifySaleCommand
        {
            SaleId = sale.Id,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = newProductId, Quantity = 2 },
                new() { ProductId = productId, Quantity = 0 }
            }
        };

        _saleRepository.Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _productGateway.Setup(p => p.GetByIdAsync(newProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(newProductId, "New Product", 20.0m));
        _productGateway.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto(productId, "New Product", 20.0m));
        _messageBus.Setup(x => x.PublishMessagesAsync(It.IsAny<IEnumerable<IntegrationMessage>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        sale.Items.First(i => i.ProductId != newProductId).IsCancelled.Should().BeTrue();
        _messageBus.Verify(x => x.PublishMessagesAsync(It.Is<IEnumerable<IntegrationMessage>>(msgs =>
            msgs.Where(x => x.Name == "sales.item.cancelled").Count() == 3 &&
            msgs.Where(x => x.Name == "sales.sale.modified").Count() == 1 &&
            msgs.Where(x => x.Name == "sales.sale.cancelled").Count() == 0 &&
            msgs.First().Payload is SaleItemCancelledEvent),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should add item to sale when product is not already in sale")]
    public async Task Handle_ShouldAddToSale_WhenProductNotInSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = SaleTestData.ValidProductId;
        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        var sale = SaleTestData.CreateValidSale();
        typeof(Sale).GetProperty("Id")?.SetValue(sale, saleId);

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);

        var productMock = new ProductDto(productId, "Product Test", 10.0m);
        _productGateway.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(productMock);

        // Act
        await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        sale.Items.Should().ContainSingle(i => i.ProductId == productId && i.Quantity == 2 && i.UnitPrice == 10.0m);
    }

    [Fact(DisplayName = "Should update product quantity when already in sale")]
    public async Task Handle_ShouldUpdateItemQuantity_WhenProductAlreadyInSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = SaleTestData.ValidProductId;
        var sale = SaleTestData.CreateSaleWithItem(2, 15.0m);
        typeof(Sale).GetProperty("Id")?.SetValue(sale, saleId);

        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 5 }
            }
        };

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _messageBus.Setup(x => x.PublishMessagesAsync(It.IsAny<IEnumerable<IntegrationMessage>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        sale.Items.First(i => i.ProductId == productId).Quantity.Should().Be(5);
        _messageBus.Verify(x => x.PublishMessagesAsync(It.Is<IEnumerable<IntegrationMessage>>(msgs =>
                msgs.Count() == 1 &&
                msgs.First().Name == "sales.sale.modified" &&
                msgs.First().Payload is SaleModifiedEvent),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should cancel sale when all items are cancelled by quantity zero")]
    public async Task Handle_ShouldCancelSale_WhenAllItemsCancelledByQuantityZero()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = SaleTestData.ValidProductId;
        var sale = SaleTestData.CreateSaleWithItem(2, 10m);
        typeof(Sale).GetProperty("Id")?.SetValue(sale, saleId);

        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new() { ProductId = productId, Quantity = 0 }
            }
        };

        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _mapper.Setup(m => m.Map<ModifySaleResult>(It.IsAny<Sale>())).Returns(new ModifySaleResult { Id = saleId });
        _messageBus.Setup(x => x.PublishMessagesAsync(It.IsAny<IEnumerable<IntegrationMessage>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.Items.All(i => i.IsCancelled).Should().BeTrue();
        result.Id.Should().Be(saleId);
        _messageBus.Verify(x => x.PublishMessagesAsync(It.Is<IEnumerable<IntegrationMessage>>(msgs =>
                msgs.Count() == 2 &&
                msgs.Where(x => x.Name == "sales.sale.cancelled" && x.Payload is SaleCancelledEvent).Count() == 1 &&
                msgs.Where(x => x.Name == "sales.item.cancelled" && x.Payload is SaleItemCancelledEvent).Count() == 1),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}