using Ambev.DeveloperEvaluation.Application.Common.Ports;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleValidator()
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

            RuleForEach(x => x.Items).SetValidator(new CreateSaleItemValidator());
        }
    }
}
