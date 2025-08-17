using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales
{
    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, List<GetSaleResult>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public GetAllSalesQueryHandler(ISaleRepository saleRepository,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<List<GetSaleResult>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            var result = await _saleRepository.GetAllSalesAsync(cancellationToken);
            return _mapper.Map<List<GetSaleResult>>(result);
        }
    }
}
