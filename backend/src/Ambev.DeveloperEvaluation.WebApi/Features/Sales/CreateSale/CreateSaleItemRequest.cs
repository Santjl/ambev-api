namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
