using Ambev.DeveloperEvaluation.Application.Common.Ports;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
    {
        public CreateSaleItemRequestValidator(IProductGateway product)
        {
            RuleFor(x => x.ProductId)
                .NotNull()
                .WithMessage("Product ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity should be greater than 0.")
                .LessThanOrEqualTo(20)
                .WithMessage("Quantity should be less or equal to 20.");

            RuleFor(i => i.ProductId)
              .MustAsync(async (productId, ct) => await product.GetByIdAsync(productId, ct) is not null)
              .WithMessage("Product does not exist.");
        }
    }
}
