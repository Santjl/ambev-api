using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales
{
    public class GetAllSalesQuery : IRequest<List<GetSaleResult>>
    {
    }
}
