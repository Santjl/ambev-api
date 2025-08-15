namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleItemCancelledEvent
    {
        public Guid SaleItemId { get; set; }

        public SaleItemCancelledEvent(Guid saleItemId)
        {
            SaleItemId = saleItemId;
        }
    }
}
