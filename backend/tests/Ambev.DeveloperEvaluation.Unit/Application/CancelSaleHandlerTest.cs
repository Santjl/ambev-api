using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CancelSaleHandlerTest
{
    private readonly Mock<ISaleRepository> _saleRepository = new();
    private readonly Mock<IMessageBus> _messageBus = new();

    private CancelSaleHandler CreateHandler() =>
        new CancelSaleHandler(_saleRepository.Object, _messageBus.Object);

    private CancelSaleCommand GetValidCommand(Guid? id = null)
    {
        return new CancelSaleCommand { Id = id ?? Guid.NewGuid() };
    }

    private Sale CreateValidSale(Guid? id = null, bool isCancelled = false)
    {
        var sale = Sale.Create(
            "SALE-001",
            DateTimeOffset.UtcNow,
            Guid.NewGuid(),
            "Customer Test",
            Guid.NewGuid(),
            "Branch Test"
        );
        typeof(Sale).GetProperty("Id")?.SetValue(sale, id ?? Guid.NewGuid());
        if (isCancelled)
            sale.Cancel();
        return sale;
    }

    [Fact(DisplayName = "Should cancel sale successfully and publish event")]
    public async Task Handle_ShouldCancelSaleSuccessfully_AndPublishEvent()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = GetValidCommand(saleId);
        var sale = CreateValidSale(saleId, isCancelled: false);

        _saleRepository.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _saleRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _messageBus.Setup(x => x.PublishEventAsync(It.IsAny<IntegrationMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sale.IsCancelled.Should().BeTrue();
        _saleRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _messageBus.Verify(x => x.PublishEventAsync(It.IsAny<IntegrationMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should not publish event if sale is already cancelled")]
    public async Task Handle_ShouldNotPublishEvent_IfSaleAlreadyCancelled()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = GetValidCommand(saleId);
        var sale = CreateValidSale(saleId, isCancelled: true);

        _saleRepository.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        _saleRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _messageBus.Verify(x => x.PublishEventAsync(It.IsAny<IntegrationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should throw ValidationException when command is invalid")]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = GetValidCommand(Guid.Empty);
        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Sale ID cannot be empty*");
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale not found")]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = GetValidCommand(saleId);

        _saleRepository.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale)null!);

        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Sale not found.");
    }
}