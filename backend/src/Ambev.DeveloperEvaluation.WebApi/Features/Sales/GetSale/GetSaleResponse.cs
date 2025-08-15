namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    public class GetSaleResponse
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
        public List<GetSaleItemResponse> Items { get; init; } = new();
    }
}
