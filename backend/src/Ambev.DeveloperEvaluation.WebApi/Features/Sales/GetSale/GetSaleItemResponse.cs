namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    public class GetSaleItemResponse
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = default!;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal DiscountPercent { get; init; }
        public decimal Total { get; init; }
        public bool IsCancelled { get; init; }
    }
}
