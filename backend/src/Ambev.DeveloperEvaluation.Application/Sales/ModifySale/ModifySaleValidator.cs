using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySale
{
    public class ModifySaleValidator : AbstractValidator<ModifySaleCommand>
    {
        public ModifySaleValidator()
        {
            RuleFor(x => x.SaleId).NotEmpty();
            RuleFor(x => x.Items).NotEmpty();

            RuleFor(x => x.Items.Select(i => i.ProductId))
                .Must(ids => ids.Distinct().Count() == ids.Count())
                .WithMessage("Duplicate ProductId in items payload.");

            RuleForEach(x => x.Items).ChildRules(cr =>
            {
                cr.RuleFor(i => i.ProductId).NotEmpty();
                cr.RuleFor(i => i.Quantity).GreaterThanOrEqualTo(0).LessThanOrEqualTo(20);
            });
        }
    }
}
