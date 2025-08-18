using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySale
{
    public class ModifySaleRequest
    {
        public List<ModifySaleItemRequest> Items { get; set; } = new();
    }
}
