using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using Xunit;
using static Ambev.DeveloperEvaluation.Application.Common.Ports.DTos;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTest
{
    private readonly Mock<ISaleRepository> _saleRepository = new();
    private readonly Mock<IProductGateway> _productGateway = new();
    private readonly Mock<ICustomerGateway> _customerGateway = new();
    private readonly Mock<IBranchGateway> _branchGateway = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IMessageBus> _messageBus = new();

    private CreateSaleHandler CreateHandler() =>
        new CreateSaleHandler(
            _saleRepository.Object,
            _productGateway.Object,
            _customerGateway.Object,
            _branchGateway.Object,
            _mapper.Object,
            _messageBus.Object);

    private CreateSaleCommand GetValidCommand()
    {
        return new CreateSaleCommand
        {
            Number = "SALE-001",
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2
                }
            }
        };
    }

    [Fact(DisplayName = "Should create sale successfully")]
    public async Task Handle_ShouldCreateSaleSuccessfully()
    {
        // Arrange
        var command = GetValidCommand();
        var customer = new CustomerDto (command.CustomerId,"Customer");
        var branch = new BranchDto (command.BranchId, "Branch");
        var product = new ProductDto (command.Items[0].ProductId, "Product", 10m);

        _customerGateway.Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _branchGateway.Setup(x => x.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _productGateway.Setup(x => x.GetByIdAsync(command.Items[0].ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var sale = Sale.Create(command.Number, DateTimeOffset.UtcNow, customer.Id, customer.Name, branch.Id, branch.Name);
        sale.AddItem(product.Id, product.Name, command.Items[0].Quantity, product.Price);

        _saleRepository.Setup(x => x.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        _mapper.Setup(x => x.Map<CreateSaleResult>(It.IsAny<Sale>()))
            .Returns(new CreateSaleResult { Id = sale.Id });

        _messageBus.Setup(x => x.PublishEventAsync(It.IsAny<SaleCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(sale.Id);
        _saleRepository.Verify(x => x.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
        _messageBus.Verify(x => x.PublishEventAsync(It.IsAny<SaleCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should throw ValidationException when command is invalid")]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = GetValidCommand();
        command.Number = "";

        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Should throw DomainException when customer not found")]
    public async Task Handle_ShouldThrowDomainException_WhenCustomerNotFound()
    {
        // Arrange
        var command = GetValidCommand();
        _customerGateway.Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerDto)null!);

        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Customer not found.");
    }

    [Fact(DisplayName = "Should throw DomainException when branch not found")]
    public async Task Handle_ShouldThrowDomainException_WhenBranchNotFound()
    {
        // Arrange
        var command = GetValidCommand();
        var customer = new CustomerDto (command.CustomerId, "Customer");
        _customerGateway.Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _branchGateway.Setup(x => x.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BranchDto)null!);

        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Branch not found.");
    }

    [Fact(DisplayName = "Should throw DomainException when product not found")]
    public async Task Handle_ShouldThrowDomainException_WhenProductNotFound()
    {
        // Arrange
        var command = GetValidCommand();
        var customer = new CustomerDto (command.CustomerId, "Customer");
        var branch = new BranchDto (command.BranchId, "Branch");
        _customerGateway.Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _branchGateway.Setup(x => x.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _productGateway.Setup(x => x.GetByIdAsync(command.Items[0].ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto)null!);

        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage($"Product {command.Items[0].ProductId} not found.");
    }
}