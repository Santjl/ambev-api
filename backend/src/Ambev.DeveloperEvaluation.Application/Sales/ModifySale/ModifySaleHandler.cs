using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySale
{
    public class ModifySaleHandler : IRequestHandler<ModifySaleCommand, ModifySaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductGateway _products;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;

        public ModifySaleHandler(ISaleRepository repo, IProductGateway products, IMapper mapper, IMessageBus messageBus)
        {
            _saleRepository = repo;
            _products = products;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        public async Task<ModifySaleResult> Handle(ModifySaleCommand request, CancellationToken cancellationToken)
        {
            var validator = new ModifySaleValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            
            if(sale is null)
                throw new KeyNotFoundException("Sale not found.");

            if (sale.IsCancelled) 
                throw new ApplicationException("Sale is cancelled.");

            var existingByProduct = sale.Items.ToDictionary(i => i.ProductId, i => i);
            var incomingByProduct = request.Items.ToDictionary(i => i.ProductId, i => i);

            var messages = new List<IntegrationMessage>();

            foreach (var incoming in request.Items)
            {
                if (existingByProduct.TryGetValue(incoming.ProductId, out var current))
                {
                    if (incoming.Quantity == 0)
                    {
                        sale.CancelItem(current.Id);
                        var message = new IntegrationMessage(EventsConsts.SaleItemCancelled, new SaleItemCancelledEvent(current.Id), DateTimeOffset.UtcNow);
                        messages.Add(message);
                        continue;
                    }

                    sale.UpdateItem(current.Id, incoming.Quantity, current.UnitPrice);
                    continue;
                }

                if (incoming.Quantity > 0)
                {
                    var product = await _products.GetByIdAsync(incoming.ProductId, cancellationToken)
                        ?? throw new ApplicationException($"Product {incoming.ProductId} not found.");

                    sale.AddItem(product.Id, product.Name, incoming.Quantity, product.Price);
                }
            }

            var incomingSet = incomingByProduct.Keys.ToHashSet();
            foreach (var item in sale.Items.Where(i => !incomingSet.Contains(i.ProductId)))
            {
                if (!item.IsCancelled)
                {
                    sale.CancelItem(item.Id);
                    var message = new IntegrationMessage(EventsConsts.SaleItemCancelled, new SaleItemCancelledEvent(item.Id), DateTimeOffset.Now);
                    messages.Add(message);
                }
            }

            if(sale.Items.Where(x => !x.IsCancelled).Count() == 0)
            {
                sale.Cancel();
                messages.Add(new IntegrationMessage(EventsConsts.SaleCancelled, new SaleCancelledEvent(sale.Id), DateTimeOffset.Now));
            }


            await _saleRepository.SaveChangesAsync(cancellationToken);
            if (!sale.IsCancelled)
            {
                messages.Add(new IntegrationMessage(EventsConsts.SaleModified, new SaleModifiedEvent(sale), DateTimeOffset.Now));
            }

            await _messageBus.PublishMessagesAsync(messages, cancellationToken);

            return _mapper.Map<ModifySaleResult>(sale);
        }
    }
}
