using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySale
{
    public class ModifySaleProfile : Profile
    {
        public ModifySaleProfile()
        {
            CreateMap<Sale, ModifySaleResult>();
        }
    }
}
