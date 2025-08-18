using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMessageBus _messageBus;
        public CancelSaleHandler(ISaleRepository saleRepository,
            IMessageBus messageBus)
        {
            _saleRepository = saleRepository;
            _messageBus = messageBus;
        }

        public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var validator = new CancelSaleValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if(sale is null)
                throw new KeyNotFoundException("Sale not found.");

            if (!sale.IsCancelled)
            {
                sale.Cancel();
                await _saleRepository.SaveChangesAsync(cancellationToken);

                var message = new IntegrationMessage(EventsConsts.SaleCancelled, new SaleCancelledEvent(sale.Id), DateTimeOffset.Now);
                await _messageBus.PublishAsync(message, cancellationToken);
            }

            return new CancelSaleResult { Success = true };
        }
    }
}
