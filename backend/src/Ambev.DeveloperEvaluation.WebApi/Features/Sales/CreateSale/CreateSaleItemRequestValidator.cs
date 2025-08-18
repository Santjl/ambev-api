using Ambev.DeveloperEvaluation.Application.Common.Ports;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
    {
        public CreateSaleItemRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotNull()
                .WithMessage("Product ID is required.");
        }
    }
}
