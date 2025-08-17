using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

public class GetAllSalesQueryHandlerTest
{
    [Fact(DisplayName = "Should return mapped sales list when repository returns sales")]
    public async Task Handle_ShouldReturnMappedSalesList_WhenRepositoryReturnsSales()
    {
        // Arrange
        var sales = new List<Sale>
        {
            Sale.Create("SALE-001", DateTimeOffset.UtcNow, Guid.NewGuid(), "Customer 1", Guid.NewGuid(), "Branch 1"),
            Sale.Create("SALE-002", DateTimeOffset.UtcNow, Guid.NewGuid(), "Customer 2", Guid.NewGuid(), "Branch 2")
        };

        var mappedResults = new List<GetSaleResult>
        {
            new GetSaleResult { Id = sales[0].Id, Number = "SALE-001" },
            new GetSaleResult { Id = sales[1].Id, Number = "SALE-002" }
        };

        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetAllSalesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(sales);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<List<GetSaleResult>>(sales)).Returns(mappedResults);

        var handler = new GetAllSalesQueryHandler(saleRepositoryMock.Object, mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetAllSalesQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(mappedResults);
        saleRepositoryMock.Verify(r => r.GetAllSalesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mapperMock.Verify(m => m.Map<List<GetSaleResult>>(sales), Times.Once);
    }

    [Fact(DisplayName = "Should return empty list when repository returns no sales")]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNoSales()
    {
        // Arrange
        var sales = new List<Sale>();
        var mappedResults = new List<GetSaleResult>();

        var saleRepositoryMock = new Mock<ISaleRepository>();
        saleRepositoryMock.Setup(r => r.GetAllSalesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(sales);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<List<GetSaleResult>>(sales)).Returns(mappedResults);

        var handler = new GetAllSalesQueryHandler(saleRepositoryMock.Object, mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetAllSalesQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        saleRepositoryMock.Verify(r => r.GetAllSalesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mapperMock.Verify(m => m.Map<List<GetSaleResult>>(sales), Times.Once);
    }
}