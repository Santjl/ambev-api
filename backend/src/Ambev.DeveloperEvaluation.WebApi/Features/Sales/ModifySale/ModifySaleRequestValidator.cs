using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySale
{
    public class ModifySaleRequestValidator : AbstractValidator<ModifySaleRequest>
    {
        public ModifySaleRequestValidator()
        {
            RuleFor(x => x.Items).NotEmpty();

            RuleFor(x => x.Items.Select(i => i.ProductId))
                .Must(ids => ids.Distinct().Count() == ids.Count())
                .WithMessage("Duplicate ProductId in items payload.");

            RuleForEach(x => x.Items).ChildRules(cr =>
            {
                cr.RuleFor(i => i.ProductId).NotEmpty();
            });
        }
    }
}
