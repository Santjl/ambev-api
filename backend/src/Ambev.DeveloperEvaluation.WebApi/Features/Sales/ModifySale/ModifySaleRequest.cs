using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySale
{
    public class ModifySaleRequest
    {
        public Guid SaleId { get; }
        public List<ModifySaleItemCommand> Items { get; } = new();
    }
}
