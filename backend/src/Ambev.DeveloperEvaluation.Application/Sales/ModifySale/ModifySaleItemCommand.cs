namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySale
{
    public class ModifySaleItemCommand
    {
        public Guid ProductId { get; set; }
        /// <summary>
        /// 0 => Cancel item
        /// > 0 => Update quantity
        /// </summary>
        public int Quantity { get; set; }
    }
}
