using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySale
{
    public class ModifySaleCommand : IRequest<ModifySaleResult>
    {
        public Guid SaleId { get; }
        public List<ModifySaleItemCommand> Items { get; } = new();
    }
}
