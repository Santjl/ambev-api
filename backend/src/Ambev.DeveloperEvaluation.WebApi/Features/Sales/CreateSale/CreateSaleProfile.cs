using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        public CreateSaleProfile()
        {
            CreateMap<CreateSaleRequest, CreateSaleCommand>()
                .ForMember(x => x.Items, dest => dest.MapFrom(x => x.Items));
            CreateMap<CreateSaleItemRequest, CreateSaleItemCommand>();
            CreateMap<CreateSaleResult, CreateSaleResponse>();
        }
    }
}
