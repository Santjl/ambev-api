namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleCancelledEvent
    {
        public Guid SaleId { get; set; }

        public SaleCancelledEvent(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}
