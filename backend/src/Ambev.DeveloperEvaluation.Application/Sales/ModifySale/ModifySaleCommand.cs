using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySale
{
    public class ModifySaleCommand : IRequest<ModifySaleResult>
    {
        public Guid SaleId { get; set; }
        public List<ModifySaleItemCommand> Items { get; set; } = new();
    }
}
