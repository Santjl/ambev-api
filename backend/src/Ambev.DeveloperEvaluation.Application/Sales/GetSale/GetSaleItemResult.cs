namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleItemResult
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
