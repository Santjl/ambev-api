namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleResult
    {
        public Guid Id { get; init; }
        public string Number { get; init; } = default!;
        public DateTimeOffset Date { get; init; }
        public Guid CustomerId { get; init; }
        public string CustomerName { get; init; } = default!;
        public Guid BranchId { get; init; }
        public string BranchName { get; init; } = default!;
        public decimal Total { get; init; }
        public bool IsCancelled { get; init; }
        public List<GetSaleItemResult> Items { get; init; } = new();
    }
}
