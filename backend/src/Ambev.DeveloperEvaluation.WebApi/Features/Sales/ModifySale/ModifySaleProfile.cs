using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySale
{
    public class ModifySaleProfile : Profile
    {
        public ModifySaleProfile()
        {
            CreateMap<ModifySaleRequest, ModifySaleCommand>()
                .ForMember(x => x.Items, dest => dest.MapFrom(x => x.Items));
            CreateMap<ModifySaleItemRequest, ModifySaleItemCommand>();
            CreateMap<ModifySaleResult, ModifySaleResponse>();
        }
    }
}
