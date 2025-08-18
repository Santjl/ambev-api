using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleRequestValidator()
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

            RuleFor(x => x.Items.Select(i => i.ProductId))
                .Must(ids => ids.Distinct().Count() == ids.Count())
                .WithMessage("Duplicate ProductId in items payload.");

            RuleForEach(x => x.Items).SetValidator(new CreateSaleItemRequestValidator());
        }
    }
}
