using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleModifiedEvent
    {
        public Sale Sale { get; set; }

        public SaleModifiedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
