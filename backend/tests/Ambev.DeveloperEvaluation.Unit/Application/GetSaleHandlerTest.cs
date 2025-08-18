using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleHandlerTest
{
    private readonly Mock<ISaleRepository> _saleRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private GetSaleHandler CreateHandler() =>
        new GetSaleHandler(_saleRepository.Object, _mapper.Object);

    [Fact(DisplayName = "Should return sale when found")]
    public async Task Handle_ShouldReturnSale_WhenFound()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();
        var query = new GetSaleQuery(sale.Id);
        var expectedResult = new GetSaleResult { Id = sale.Id };

        _saleRepository.Setup(x => x.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _mapper.Setup(x => x.Map<GetSaleResult>(sale))
            .Returns(expectedResult);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
        _saleRepository.Verify(x => x.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<GetSaleResult>(sale), Times.Once);
    }

    [Fact(DisplayName = "Should throw ValidationException when query is invalid")]
    public async Task Handle_ShouldThrowValidationException_WhenQueryIsInvalid()
    {
        // Arrange
        var query = new GetSaleQuery(Guid.Empty);
        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Sale ID is required*");
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale not found")]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleNotFound()
    {
        // Arrange
        var query = new GetSaleQuery(Guid.NewGuid());
        _saleRepository.Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale)null!);

        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {query.Id} not found");
    }
}