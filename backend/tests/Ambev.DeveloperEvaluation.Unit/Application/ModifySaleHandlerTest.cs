using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;
using static Ambev.DeveloperEvaluation.Application.Common.Ports.DTos;

public class ModifySaleHandlerTest
{
    [Fact(DisplayName = "Should be modify sale with success when command data is valid")]
    public async Task Handle_ShouldModifySale_WhenValidCommand()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = ModifySaleHandlerTestData.GenerateValidCommand(saleId);
        var saleRepositoryMock = new Mock<ISaleRepository>();
        var productGatewayMock = new Mock<IProductGateway>();
        var mapperMock = new Mock<IMapper>();
        var messageBusMock = new Mock<IMessageBus>();

        var saleMock = new Mock<Sale>();
        saleMock.Setup(s => s.IsCancelled).Returns(false);
        saleMock.Setup(s => s.Items).Returns(new List<SaleItem>());
        saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(saleMock.Object);

        mapperMock.Setup(m => m.Map<ModifySaleResult>(It.IsAny<object>())).Returns(new ModifySaleResult { Id = saleId });

        var handler = new ModifySaleHandler(saleRepositoryMock.Object, productGatewayMock.Object, mapperMock.Object, messageBusMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(saleId);
        saleRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        messageBusMock.Verify(m => m.PublishEventAsync(It.IsAny<IntegrationMessage>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact(DisplayName = "Should throw exception when SaleId is not found.")]
    public async Task Handle_ShouldThrow_WhenSaleIdDoesNotExist()
    {
        // Arrange
        var command = ModifySaleHandlerTestData.GenerateCommandWithNonExistentSaleId();
        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>())).ReturnsAsync((Sale)null);

        var handler = new ModifySaleHandler(saleRepositoryMock.Object, Mock.Of<IProductGateway>(), Mock.Of<IMapper>(), Mock.Of<IMessageBus>());

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw exception when try to modify cancelled sale")]
    public async Task Handle_ShouldThrowException_WhenSaleIsCancelled()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = ModifySaleHandlerTestData.GenerateCommandForCancelledSale(saleId);
        var saleMock = new Mock<Sale>();
        saleMock.Setup(s => s.IsCancelled).Returns(true);
        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(saleMock.Object);

        var handler = new ModifySaleHandler(saleRepositoryMock.Object, Mock.Of<IProductGateway>(), Mock.Of<IMapper>(), Mock.Of<IMessageBus>());

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw exception when product does now exist")]
    public async Task Handle_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = ModifySaleHandlerTestData.GenerateCommandWithNonExistentProduct(saleId);
        var saleMock = new Mock<Sale>();
        saleMock.Setup(s => s.IsCancelled).Returns(false);
        saleMock.Setup(s => s.Items).Returns(new List<SaleItem>());
        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(saleMock.Object);

        var productGatewayMock = new Mock<IProductGateway>();
        productGatewayMock.Setup(p => p.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto)null!);

        var handler = new ModifySaleHandler(saleRepositoryMock.Object, productGatewayMock.Object, Mock.Of<IMapper>(), Mock.Of<IMessageBus>());

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should cancel item when quantity is update to zero.")]
    public async Task Handle_ShouldCancelItem_WhenQuantityZero()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = ModifySaleHandlerTestData.GenerateCommandToCancelItem(saleId, productId);

        var saleItemMock = new Mock<SaleItem>();
        saleItemMock.SetupGet(i => i.ProductId).Returns(productId);
        saleItemMock.SetupGet(i => i.Id).Returns(Guid.NewGuid());
        saleItemMock.SetupGet(i => i.IsCancelled).Returns(false);

        var saleMock = new Mock<Sale>();
        saleMock.Setup(s => s.IsCancelled).Returns(false);
        saleMock.Setup(s => s.Items).Returns(new List<SaleItem> { saleItemMock.Object });

        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(saleMock.Object);

        var handler = new ModifySaleHandler(saleRepositoryMock.Object, Mock.Of<IProductGateway>(), Mock.Of<IMapper>(), Mock.Of<IMessageBus>());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        saleMock.Verify(s => s.CancelItem(saleItemMock.Object.Id), Times.Once);
    }

    [Fact(DisplayName = "Should add item to sale when product is not already in sale")]
    public async Task Handle_ShouldAddToSale_WhenProductNotInSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new ModifySaleItemCommand { ProductId = productId, Quantity = 2 }
            }
        };

        var saleMock = new Mock<Sale>();
        saleMock.Setup(s => s.IsCancelled).Returns(false);
        saleMock.Setup(s => s.Items).Returns(new List<SaleItem>());

        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(saleMock.Object);

        var productMock = new ProductDto(productId, "Produto Teste", 10.0m);

        var productGatewayMock = new Mock<IProductGateway>();
        productGatewayMock.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(productMock);

        var mapperMock = new Mock<IMapper>();
        var messageBusMock = new Mock<IMessageBus>();

        var handler = new ModifySaleHandler(saleRepositoryMock.Object, productGatewayMock.Object, mapperMock.Object, messageBusMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        saleMock.Verify(s => s.AddItem(productId, "Produto Teste", 2, 10.0m), Times.Once);
    }

    [Fact(DisplayName = "Should update product quantity when already in sale")]
    public async Task Handle_ShouldUpdateItemQuantity_WhenProductAlreadyInSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var saleItemId = Guid.NewGuid();
        var command = new ModifySaleCommand
        {
            SaleId = saleId,
            Items = new List<ModifySaleItemCommand>
            {
                new ModifySaleItemCommand { ProductId = productId, Quantity = 5 }
            }
        };

        var saleItemMock = new Mock<SaleItem>();
        saleItemMock.SetupGet(i => i.ProductId).Returns(productId);
        saleItemMock.SetupGet(i => i.Id).Returns(saleItemId);
        saleItemMock.SetupGet(i => i.UnitPrice).Returns(15.0m);
        saleItemMock.SetupGet(i => i.IsCancelled).Returns(false);

        var saleMock = new Mock<Sale>();
        saleMock.Setup(s => s.IsCancelled).Returns(false);
        saleMock.Setup(s => s.Items).Returns(new List<SaleItem> { saleItemMock.Object });

        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>())).ReturnsAsync(saleMock.Object);

        var mapperMock = new Mock<IMapper>();
        var messageBusMock = new Mock<IMessageBus>();

        var handler = new ModifySaleHandler(saleRepositoryMock.Object, Mock.Of<IProductGateway>(), mapperMock.Object, messageBusMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        saleMock.Verify(s => s.UpdateItem(saleItemId, 5, 15.0m), Times.Once);
    }
}