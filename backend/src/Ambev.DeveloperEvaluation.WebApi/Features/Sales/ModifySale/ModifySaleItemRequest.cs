namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySale
{
    public class ModifySaleItemRequest
    {
        public Guid ProductId { get; set; }
        /// <summary>
        /// 0 => Cancel item
        /// > 0 => Update quantity
        /// </summary>
        public int Quantity { get; set; }
    }
}
