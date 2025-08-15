using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleRequestValidator(
                IBranchGateway branches,
                ICustomerGateway customers,
                IProductGateway product
            )
        {
            RuleFor(x => x.Number)
                .NotEmpty()
                .WithMessage("Sale number is required.");

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required.");

            RuleFor(x => x.BranchId)
                .NotEmpty()
                .WithMessage("Branch ID is required.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one sale item is required.");

            RuleFor(x => x.CustomerId)
            .MustAsync(async (id, ct) => await customers.GetByIdAsync(id, ct) is not null)
            .WithMessage("Customer does not exist.");

            RuleFor(x => x.BranchId)
                .MustAsync(async (id, ct) => await branches.GetByIdAsync(id, ct) is not null)
                .WithMessage("Branch does not exist.");

            RuleForEach(x => x.Items).SetValidator(new CreateSaleItemRequestValidator(product));
        }
    }
}
