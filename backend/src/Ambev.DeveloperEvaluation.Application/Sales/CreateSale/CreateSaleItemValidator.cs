using Ambev.DeveloperEvaluation.Application.Common.Ports;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemCommand>
    {
        public CreateSaleItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotNull()
                .WithMessage("Product ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity should be greater than 0.")
                .LessThanOrEqualTo(20)
                .WithMessage("Quantity should be less or equal to 20.");
        }
    }
}
